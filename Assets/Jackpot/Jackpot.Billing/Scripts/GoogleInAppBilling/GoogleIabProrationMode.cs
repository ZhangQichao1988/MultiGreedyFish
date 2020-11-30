using System;
using System.Collections.Generic;

namespace Jackpot.Billing
{
    public enum GoogleIabProrationMode
    {
        /**
         * 比例分配モード：IMMEDIATE_WITH_TIME_PRORATION
         * 切り替えは直ちに有効になり、新しい有効期限が比例配分され、ユーザーへの入金または請求が行われます。
         * Google側現行のデフォルト動作は「ImmediateWithTimeProration」である
         */
        ImmediateWithTimeProration,

        /**
         * 比例分配モード：IMMEDIATE_AND_CHARGE_PRORATED_PRICE
         * 切り替えは直ちに有効になりますが、請求期間は変わりません。残りの期間の価格に対する請求が行われます。
         * ※ このオプションは定期購入のアップグレードでのみ利用可能です
         */
        ImmediateAndChargeProratedPrice,

        /**
         * 比例分配モード：IMMEDIATE_WITHOUT_PRORATION,
         * 切り替えは直ちに有効になり、新しい価格が次回契約期間に請求されます。請求期間は変わりません。
         */
        ImmediateWithoutProration,

        /**
         * 比例分配モード：DEFERRED
         * ユーザーは次回の請求日まで、以前の定期購入にアクセスできます。
         */
        Deferred,
    }
}
