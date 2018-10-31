using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace Website.Service.Stores
{
    internal static class StoreHelpers
    {
        internal static PropertyCheckResult CheckIfPropertyExists(string sortPropName, params Type[] types)
        {
            foreach (var type in types)
            {
                var typeProps = type.GetProperties(System.Reflection.BindingFlags.Public
                                                   | System.Reflection.BindingFlags.Instance
                                                   | System.Reflection.BindingFlags.DeclaredOnly)
                    .Select(x => x.Name).ToArray();
                if (typeProps.Contains(sortPropName))
                    return new PropertyCheckResult(true, type);
            }
            return new PropertyCheckResult(false);
        }

        internal static ChangeResult<TSource> ChangeCompare<TSource, TKey>(IEnumerable<TSource> local, IEnumerable<TSource> remote, Func<TSource, TKey> keySelector)
        {
            if (local == null)
                throw new ArgumentNullException(nameof(local));
            if (remote == null)
                throw new ArgumentNullException(nameof(remote));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            var remoteKeyValues = remote.ToDictionary(keySelector);

            var deleted = new List<TSource>();
            var changed = new List<TSource>();
            var localKeys = new HashSet<TKey>();

            foreach (var localItem in local)
            {
                var localKey = keySelector(localItem);
                localKeys.Add(localKey);

                /* Check if primary key exists in both local and remote 
                 * and if so check if changed, if not it has been deleted
                 */
                if (remoteKeyValues.TryGetValue(localKey, out var changeCandidate))
                {
                    if (!changeCandidate.Equals(localItem))
                        changed.Add(changeCandidate);
                }
                else
                {
                    deleted.Add(localItem);
                }
            }
            var inserted = remoteKeyValues
                .Where(x => !localKeys.Contains(x.Key))
                .Select(x => x.Value)
                .ToList();

            return new ChangeResult<TSource>(deleted, changed, inserted);
        }

        /// <summary>
        /// Immutable class containing changes
        /// </summary>
        internal sealed class ChangeResult<T>
        {
            public ChangeResult(IList<T> deleted, IList<T> changed, IList<T> inserted)
            {
                Deleted = new ReadOnlyCollection<T>(deleted);
                Changed = new ReadOnlyCollection<T>(changed);
                Inserted = new ReadOnlyCollection<T>(inserted);
            }

            public IList<T> Deleted { get; private set; }
            public IList<T> Changed { get; private set; }
            public IList<T> Inserted { get; private set; }
        }

        internal class PropertyCheckResult
        {
            public Type Type;
            public bool Result;

            public PropertyCheckResult(bool result, Type type = null)
            {
                Type = type;
                Result = result;
            }
        }

        /// <summary>
        /// Scale image proportionally with max
        /// </summary>
        /// <param name="image">Input image</param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        internal static Bitmap ScaleImage(Bitmap image, int maxWidth, int maxHeight)
        {
            // check if image already fits
            if (image.Width < maxWidth && image.Height < maxHeight)
                return image;

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        internal static System.Drawing.Imaging.ImageFormat GetImageFormat(this System.Drawing.Image img)
        {
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                return System.Drawing.Imaging.ImageFormat.Bmp;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                return System.Drawing.Imaging.ImageFormat.Png;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Emf))
                return System.Drawing.Imaging.ImageFormat.Emf;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Exif))
                return System.Drawing.Imaging.ImageFormat.Exif;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                return System.Drawing.Imaging.ImageFormat.Gif;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Icon))
                return System.Drawing.Imaging.ImageFormat.Icon;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.MemoryBmp))
                return System.Drawing.Imaging.ImageFormat.MemoryBmp;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff))
                return System.Drawing.Imaging.ImageFormat.Tiff;
            else
                return System.Drawing.Imaging.ImageFormat.Wmf;
        }
    }
}
