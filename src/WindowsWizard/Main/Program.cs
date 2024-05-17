global using System;

using System.IO;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using FiltersForge.CUI;
using FiltersForge.Filters.Common.Base;
using FiltersForge.Filters.Common.Manager;
using FiltersForge.Filters.Common.Validation;

namespace FiltersForge.Main
{
    internal sealed class Program
    {
        private static Bitmap? image;

        private static readonly MenuBuilder menuBuilder = new MenuBuilder();

        private static readonly Menu main = InitializeMainMenu();
        private static readonly Menu forge = InitializeForgeMenu();

        [STAThread]
        private static void Main()
        {
            const string RELATIVE_PATH_ASCII_ART = "SplashScreen.txt";

            ConsoleHelper.LoadAsciiArt(RELATIVE_PATH_ASCII_ART);

            ConsoleHelper.Start();

            while (!ConsoleHelper.ShouldBeTerminated)
            {
                ConsoleHelper.ShowMenu(Program.main);

                MenuOption option = ConsoleHelper.ReadMenuInput(Program.main, Program.HandleInvalidInput);
                option.OptionAction?.Invoke();
            }

            ConsoleHelper.Shutdown();
        }

        private static void HandleInvalidInput()
        {
            ConsoleHelper.ClearCurrentLine();
            MessageBox.Show("Введіть коректне значення.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static Menu InitializeMainMenu()
        {
            return Program.menuBuilder.AddItem(new MenuOption("Зображення", Program.ProcessImageOption))
                                      .AddItem(new MenuOption("Інформація", Program.ProcessAboutOption))
                                      .AddItem(new MenuOption("Вийти", ConsoleHelper.Shutdown))
                                      .AddTitle("Головне меню")
                                      .Build();
        }

        private static Menu InitializeForgeMenu()
        {
            foreach (string alias in FiltersManager.Instance.FiltersAliases)
            {
                if (FiltersManager.Instance[alias] != null)
                {
                    Program.menuBuilder.AddItem(alias, () => Program.ProcessFilterOption(alias));
                }
            }

            Program.menuBuilder.AddTitle("Меню вибору ефекту");

            Menu menu = Program.menuBuilder.Build();

            return menu;
        }

        private static void ProcessImageOption()
        {
            Program.ShowOpenImageFileDialog();

            if (Program.image == null)
            {
                return;
            }

            ConsoleHelper.ShowMenu(Program.forge);

            MenuOption option = ConsoleHelper.ReadMenuInput(Program.forge, Program.HandleInvalidInput);
            option.OptionAction?.Invoke();
        }

        private static void ShowOpenImageFileDialog()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Оберіть зображення";
                openFileDialog.Filter = "|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Program.image = new Bitmap(openFileDialog.FileName);
                    Console.Title = Path.GetFileName(openFileDialog.FileName);
                }
            }
        }

        private static void ProcessAboutOption()
        {
            MessageBox.Show("ІП-14 Бабіч Денис.\nКурсова робота з технологій паралельних обчислень.\nТема: Алгоритм обробки графічних зображень.", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void ProcessFilterOption(string alias)
        {
            ConsoleHelper.ClearOutputBuffer();

            Filter? filter = FiltersManager.Instance[alias];

            if (filter != null)
            {
                if (filter is IFilter)
                {
                    Program.image = ((IFilter)filter).Apply(Program.image!);
                }
                else
                {
                    Program.ProcessParameterizedFilter(filter);
                }
            }

            Program.ShowSaveImageFileDialog();
        }

        private static void ProcessParameterizedFilter(Filter filter)
        {
            Type? parameterizedInterface = filter.GetType().GetInterfaces().FirstOrDefault(type => type.GetGenericTypeDefinition() == typeof(IFilterParameterized<>));

            if (parameterizedInterface == null)
            {
                throw new TypeInitializationException(nameof(parameterizedInterface.FullName), new NotImplementedException("Filter is not implemented correcty."));
            }

            Attribute? rangeAttribute;
            IEnumerable<Attribute> attributes;
            Type preferencesType = parameterizedInterface.GetGenericArguments()[0];
            PropertyInfo[] properties = preferencesType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            object? preferences = Activator.CreateInstance(preferencesType);

            if (preferences == null)
            {
                throw new TypeInitializationException(nameof(parameterizedInterface.FullName), new TypeLoadException("Failed to instatiate preferences"));
            }

            foreach (PropertyInfo property in properties)
            {
                attributes = property.GetCustomAttributes();
                rangeAttribute = attributes.FirstOrDefault(attribute => attribute.GetType().IsGenericType && attribute.GetType().GetGenericTypeDefinition() == typeof(RangeAttribute<>));

                if (rangeAttribute == null)
                {
                    throw new TypeInitializationException(nameof(parameterizedInterface.FullName), new NotImplementedException("Filter is not implemented correcty."));
                }

                Program.ReadPreferencesInput(property, rangeAttribute, preferences);
            }

            Program.ApplyParameterizedFilter(filter, parameterizedInterface, preferences);
        }

        private static void ReadPreferencesInput(PropertyInfo property, Attribute attribute, object preferences)
        {
            const string IDENTIFIER_METHOD_IS_VALID = "IsValid";
            const string IDENTIFIER_PROPERTY_ERROR_MESSAGE = "ErrorMessage";

            Type attributeType = attribute.GetType();
            MethodInfo isValidMethod = attributeType.GetMethod(IDENTIFIER_METHOD_IS_VALID, BindingFlags.NonPublic | BindingFlags.Instance)!;
            string errorMessage = attributeType.GetProperty(IDENTIFIER_PROPERTY_ERROR_MESSAGE, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(attribute, null)!.ToString()!;

            string input;
            bool isValid;
            object value;

            Console.WriteLine($"{property.Name}: ");

            do
            {
                try
                {
                    input = ConsoleHelper.ReadInput();
                    value = Convert.ChangeType(input, property.PropertyType);

                    isValid = (bool)isValidMethod.Invoke(attribute, new[] { value })!;

                    if (isValid)
                    {
                        property.SetValue(preferences, value);
                    }
                    else
                    {
                        ConsoleHelper.ClearCurrentLine();
                        MessageBox.Show(errorMessage.ToString(), "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (FormatException)
                {
                    isValid = false;
                    ConsoleHelper.ClearCurrentLine();
                    MessageBox.Show($"Некоректне значення для параметра {property.Name}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } while (!isValid);
        }

        private static void ApplyParameterizedFilter(Filter filter, Type parameterizedInterface, object preferences)
        {
            const string FORMAT = "0.000";
            const int MILLISECONDS_IN_SECOND = 1000;
            const string IDENTIFIER_METHOD_APPLY = "Apply";

            Stopwatch stopwatch = new Stopwatch();

            MethodInfo applyMethod = parameterizedInterface!.GetMethod(IDENTIFIER_METHOD_APPLY)!;

            stopwatch.Start();

            Program.image = (Bitmap)applyMethod.Invoke(filter, new object[] { Program.image!, preferences })!;

            stopwatch.Stop();

            double elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
            double elapsedSeconds = elapsedMilliseconds / MILLISECONDS_IN_SECOND;

            MessageBox.Show($"Час виконання: {elapsedSeconds.ToString(FORMAT)} с ({elapsedMilliseconds.ToString(FORMAT)} мс).", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void ShowSaveImageFileDialog()
        {
            ConsoleHelper.ClearOutputBuffer();

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Зберегти зображення";
                saveFileDialog.Filter = "JPEG files (*.jpg;*.jpeg)|*.jpg;*.jpeg|PNG files (*.png)|*.png|Bitmap files (*.bmp)|*.bmp";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(saveFileDialog.FileName))
                    {
                        File.Delete(saveFileDialog.FileName);
                    }

                    Program.image?.Save(saveFileDialog.FileName);
                    Program.image?.Dispose();
                    Program.image = null;
                }
            }
        }
    }
}