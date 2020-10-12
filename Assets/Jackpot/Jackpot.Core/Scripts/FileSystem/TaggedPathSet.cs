/**
 * Jackpot
 * Copyright(c) 2014 KLab, Inc. All Rights Reserved.
 * Proprietary and Confidential - This source code is not for redistribution
 * 
 * Subject to the prior written consent of KLab, Inc(Licensor) and its terms and
 * conditions, Licensor grants to you, and you hereby accept nontransferable,
 * nonexclusive limited right to access, obtain, use, copy and/or download
 * a copy of this product only for requirement purposes. You may not rent,
 * lease, loan, time share, sublicense, transfer, make generally available,
 * license, disclose, disseminate, distribute or otherwise make accessible or
 * available this product to any third party without the prior written approval
 * of Licensor. Unauthorized copying of this product, including modifications
 * of this product or programs in which this product has been merged or included
 * with other software products is expressly forbidden.
 */
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Jackpot
{
    /// <summary>
    /// FileSystem.RemoveByTagsで返却される、tagとtagに紐付いていたpathのセットです。
    /// </summary>
    public class TaggedPathSet
    {
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public string Tag { get; private set; }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.TaggedPathSet"/> class.
        /// </summary>
        /// <param name="tag">Tag.</param>
        /// <param name="path">Path.</param>
        public TaggedPathSet(string tag, string path)
        {
            Tag = tag;
            Path = path;
        }
    }
}

