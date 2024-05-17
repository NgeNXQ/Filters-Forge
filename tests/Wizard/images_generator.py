import zlib
import random
import struct

def generate_large_image(output_image_path, width, height):
    with open(output_image_path, 'wb') as stream:
        stream.write(b'\x89PNG\r\n\x1a\n')
        
        ihdr_data = struct.pack('!IIBBBBB', width, height, 8, 2, 0, 0, 0)
        stream.write(struct.pack('!I', len(ihdr_data)))
        stream.write(b'IHDR')
        stream.write(ihdr_data)
        crc = zlib.crc32(b'IHDR' + ihdr_data) & 0xFFFFFFFF
        stream.write(struct.pack('!I', crc))

        compressor = zlib.compressobj()

        for _ in range(height):
            pixel_data = b''
            for _ in range(width):
                pixel_data += struct.pack('!BBB', random.randint(0, 255), random.randint(0, 255), random.randint(0, 255))

            compressed_pixel_data = compressor.compress(pixel_data)

            if compressed_pixel_data:
                stream.write(struct.pack('!I', len(compressed_pixel_data)))
                stream.write(b'IDAT')
                stream.write(compressed_pixel_data)
                crc = zlib.crc32(b'IDAT' + compressed_pixel_data) & 0xFFFFFFFF
                stream.write(struct.pack('!I', crc))

        compressed_pixel_data = compressor.flush()

        if compressed_pixel_data:
            stream.write(struct.pack('!I', len(compressed_pixel_data)))
            stream.write(b'IDAT')
            stream.write(compressed_pixel_data)
            crc = zlib.crc32(b'IDAT' + compressed_pixel_data) & 0xFFFFFFFF
            stream.write(struct.pack('!I', crc))

        stream.write(struct.pack('!I', 0))
        stream.write(b'IEND')
        crc = zlib.crc32(b'IEND') & 0xFFFFFFFF
        stream.write(struct.pack('!I', crc))

    print("Done.")

width = 20_000
height = 20_000
output_image_path = "20k.png"

generate_large_image(output_image_path, width, height)
