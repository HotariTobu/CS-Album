using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CS_Album
{
    public class HelpPageViewModel : SharedWPF.ViewModelBase
    {
        public string OperationVideoURL => "https://youtu.be/myLV4gfCDNM";
        public ImageSource OperationVideoQR => GetQR(25, 387137481587325953, 22827257270711621, 26768147462854213, -9151379318293637887, -9191006408837381327, -9199667547555165159, -9208002569390464031, -9159011288345368359, -9189845730448034784, -9191742946346023679, 39679474123421509, -9160192460186810043, 14423553);

        private Assembly Assembly { get; }
        public string AssemblyVersion =>(Assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute)?.InformationalVersion;
        public string FrameworkVersion => (Assembly.GetCustomAttribute(typeof(TargetFrameworkAttribute)) as TargetFrameworkAttribute)?.FrameworkName;
        public string Copyright => (Assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute)?.Copyright;
        public string MailAddress => "mailto:hotari13port@gmail.com";
        public ImageSource MailQR => GetQR(25, 459288534138804225, 5967752488446684485, 1828396015318839365, -7422096017429867263, 5124994888019833616, 22254554753286649, 25560998589909628, -9219420690618746008, 32095843054211957, 6734695076600350209, -868686457206330043, -4325023007858672827, 676476417);

        public HelpPageViewModel()
        {
            Assembly = Assembly.GetExecutingAssembly();
        }

        private static ImageSource GetQR(int size, params byte[] data)
        {
            WriteableBitmap bitmap = new WriteableBitmap(size, size, 96, 96, PixelFormats.BlackWhite, null);
            bitmap.WritePixels(new Int32Rect(0, 0, size, size), data, 4, 0, 0);
            return bitmap;
        }

        private static ImageSource GetQR(int size, params long[] data)
        {
            byte[] vs = new byte[data.Length * 8];
            for (int i = 0; i < data.Length; i++)
            {
                long value = data[i];
                for (int j = 0; j < 8; j++)
                {
                    vs[i * 8 + j] = (byte)(value >> (8 * j));
                }
            }
            return GetQR(size, vs);
        }
    }
}
