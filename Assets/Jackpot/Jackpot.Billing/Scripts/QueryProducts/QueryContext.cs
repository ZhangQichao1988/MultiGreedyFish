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
    public class QueryContext : Context
    {
        public QueryRequestDelegate QueryRequest { get { return queryRequest; } }

        public QueryItemsRequestDelegate QueryItemsRequest { get { return queryItemsRequest; } }

        public SuccessQueryDelegate SuccessQuery { get { return successQuery; } }

        public FailureQueryDelegate FailureQuery { get { return failureQuery; } }

        readonly QueryRequestDelegate queryRequest;
        readonly QueryItemsRequestDelegate queryItemsRequest;
        readonly SuccessQueryDelegate successQuery;
        readonly FailureQueryDelegate failureQuery;

        public QueryContext() : this(null, null, null, null)
        {
        }

        public QueryContext(
            QueryRequestDelegate queryRequest,
            SuccessQueryDelegate successQuery,
            FailureQueryDelegate failureQuery) : this(queryRequest, null, successQuery, failureQuery)
        {
        }

        public QueryContext(
            QueryItemsRequestDelegate queryItemsRequest,
            SuccessQueryDelegate successQuery,
            FailureQueryDelegate failureQuery) : this(null, queryItemsRequest, successQuery, failureQuery)
        {
        }

        QueryContext(
            QueryRequestDelegate queryRequest,
            QueryItemsRequestDelegate queryItemsRequest,
            SuccessQueryDelegate successQuery,
            FailureQueryDelegate failureQuery) : base()
        {
            this.queryRequest = queryRequest;
            this.queryItemsRequest = queryItemsRequest;
            this.successQuery = successQuery;
            this.failureQuery = failureQuery;
        }

        QueryContext(
            int id,
            QueryRequestDelegate queryRequest,
            QueryItemsRequestDelegate queryItemsRequest,
            SuccessQueryDelegate successQuery,
            FailureQueryDelegate failureQuery) : base(id)
        {
            this.queryRequest = queryRequest;
            this.queryItemsRequest = queryItemsRequest;
            this.successQuery = successQuery;
            this.failureQuery = failureQuery;
        }

        public QueryContext OnQueryRequest(QueryRequestDelegate queryRequestDelegate)
        {
            return new QueryContext(Id, queryRequestDelegate, null, SuccessQuery, FailureQuery);
        }

        public QueryContext OnQueryRequest(QueryItemsRequestDelegate queryItemsRequestDelegate)
        {
            return new QueryContext(Id, null, queryItemsRequestDelegate, SuccessQuery, FailureQuery);
        }

        public QueryContext OnSuccess(SuccessQueryDelegate successQueryDelegate)
        {
            return new QueryContext(Id, QueryRequest, QueryItemsRequest, successQueryDelegate, FailureQuery);
        }

        public QueryContext OnFailure(FailureQueryDelegate failureQueryDelegate)
        {
            return new QueryContext(Id, QueryRequest, QueryItemsRequest, SuccessQuery, failureQueryDelegate);
        }

        public override void Execute()
        {
            BillingService.Instance.Query(this);
        }
    }
}
