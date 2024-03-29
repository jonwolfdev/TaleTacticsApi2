﻿namespace HorrorTacticsApi2.Data.Entities
{
    public class FormatHelper
    {
        // TODO: Test that they both match (key vs FormatHelper.Format)
        public static readonly IReadOnlyDictionary<string, FormatHelper> AllowedImageExtensionsForUpload = new Dictionary<string, FormatHelper>()
        {
            { "." + FileFormatEnum.JPEG.ToString().ToLowerInvariant(), new FormatHelper(FileFormatEnum.JPEG, new List<byte[]>()
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xDB },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xEE },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                })
            },
            { "." + FileFormatEnum.JPG.ToString().ToLowerInvariant(), new FormatHelper(FileFormatEnum.JPG, new List<byte[]>()
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xDB },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xEE },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                })
            },
            { "." + FileFormatEnum.PNG.ToString().ToLowerInvariant(), new FormatHelper(FileFormatEnum.PNG, new List<byte[]>()
                {
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
                })
            }
        };

        public static readonly IReadOnlyDictionary<string, FormatHelper> AllowedAudioExtensionsForUpload = new Dictionary<string, FormatHelper>()
        {
            { "." + FileFormatEnum.MP3.ToString().ToLowerInvariant(), new FormatHelper(FileFormatEnum.MP3, new List<byte[]>()
                {
                    new byte[] { 0x49, 0x44, 0x33 },
                    new byte[] { 0xFF, 0xFB },
                    new byte[] { 0xFF, 0xF3 },
                    new byte[] { 0xFF, 0xF2 }
                })
            }
        };

        public FileFormatEnum Format { get; }
        public IReadOnlyList<byte[]> FileSignatures { get; }
        public FormatHelper(FileFormatEnum format, IReadOnlyList<byte[]> signature)
        {
            Format = format;
            FileSignatures = signature;
        }
    }
}
