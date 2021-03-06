﻿using CmsData.Finance.TransNational.Core;

namespace CmsData.Finance.TransNational.Transaction
{
    internal abstract class TransactRequest : Request
    {
        private const string URL = "https://secure.networkmerchants.com/api/transact.php";

        protected TransactRequest(string userName, string password) : base(URL, userName, password) {}

        public new TransactResponse Execute()
        {
            var response = base.Execute();
            return new TransactResponse(response);
        }
    }
}