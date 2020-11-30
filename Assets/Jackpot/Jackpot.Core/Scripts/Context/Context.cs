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
namespace Jackpot
{
    /// <summary>
    /// 実行コンテキストの抽象クラスです
    /// </summary>
    public abstract class Context
    {
        #region Properties

        /// <summary>
        /// Contextを紐づけるIDを示します
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get { return id; } }

        #endregion

        #region Fields

        static int serial = 0;

        readonly int id;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Context"/> class.
        /// </summary>
        protected Context()
            : this(++serial) // int.MaxValueもアプリ起動しながら処理を実施する事はないでしょう
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jackpot.Context"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        protected Context(int id)
        {
            this.id = id;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Execute this instance.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Jackpot.Context"/>.
        /// </summary>
        /// <param name="that">The <see cref="System.Object"/> to compare with the current <see cref="Jackpot.Context"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Jackpot.Context"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object that)
        {
            if (that == null)
            {
                return false;
            }
            var thatContext = that as Context;
            if (thatContext == null)
            {
                return false;
            }
            return id == thatContext.Id;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Jackpot.Context"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        #endregion

    }
}
