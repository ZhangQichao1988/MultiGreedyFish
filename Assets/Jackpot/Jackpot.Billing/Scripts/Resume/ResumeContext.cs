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

using System;
using System.Collections.Generic;

namespace Jackpot.Billing
{
    public class ResumeContext : Context
    {
        public SuccessResumeDelegate SuccessResume { get { return successResume; } }

        public FailureResumeDelegate FailureResume { get { return failureResume; } }

        readonly SuccessResumeDelegate successResume;
        readonly FailureResumeDelegate failureResume;

        public ResumeContext() : this(null, null)
        {
        }

        public ResumeContext(
            SuccessResumeDelegate successRestore,
            FailureResumeDelegate failureRestore) : base()
        {
            this.successResume = successRestore;
            this.failureResume = failureRestore;
        }

        ResumeContext(
            int id,
            SuccessResumeDelegate successResume,
            FailureResumeDelegate failureResume) : base(id)
        {
            this.successResume = successResume;
            this.failureResume = failureResume;
        }

        public ResumeContext OnSuccessResume(SuccessResumeDelegate successResumeDelegate)
        {
            return new ResumeContext(
                Id,
                successResumeDelegate,
                FailureResume
            );
        }

        public ResumeContext OnFailureResume(FailureResumeDelegate failureResumeDelegate)
        {
            return new ResumeContext(
                Id,
                SuccessResume,
                failureResumeDelegate
            );
        }

        public override void Execute()
        {
            BillingService.Instance.Resume(this);
        }
    }
}
