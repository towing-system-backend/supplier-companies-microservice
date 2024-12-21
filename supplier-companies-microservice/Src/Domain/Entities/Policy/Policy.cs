using Application.Core;

namespace SupplierCompany.Domain
{
    public class Policy(PolicyId id, PolicyTitle title, PolicyCoverageAmount coverageAmount, PolicyPrice price, PolicyType type, PolicyIssuanceDate issuanceDate, PolicyExpirationDate expirationDate) : Entity<PolicyId>(id)
    {
        private readonly PolicyId _id = id;
        private readonly PolicyTitle _title = title;
        private readonly PolicyCoverageAmount _coverageAmount = coverageAmount;
        private readonly PolicyPrice _price = price;
        private readonly PolicyType _type = type;
        private readonly PolicyIssuanceDate _issuanceDate = issuanceDate;
        private readonly PolicyExpirationDate _expirationDate = expirationDate;

        public PolicyId GetId() => _id;
        public PolicyTitle GetTitle() => _title;
        public PolicyCoverageAmount GetCoverageAmount() => _coverageAmount;
        public PolicyPrice GetPrice() => _price;
        public PolicyType GetType() => _type;
        public PolicyIssuanceDate GetIssuanceDate() => _issuanceDate;
        public PolicyExpirationDate GetExpirationDate() => _expirationDate;
    }
}