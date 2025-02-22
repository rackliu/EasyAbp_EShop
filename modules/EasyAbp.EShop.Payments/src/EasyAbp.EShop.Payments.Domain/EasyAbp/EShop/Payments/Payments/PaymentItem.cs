using System;
using EasyAbp.EShop.Stores.Stores;
using EasyAbp.PaymentService.Payments;
using JetBrains.Annotations;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.DynamicProxy;

namespace EasyAbp.EShop.Payments.Payments
{
    public class PaymentItem : FullAuditedEntity<Guid>, IPaymentItem, IMultiStore
    {
        #region Base properties

        [NotNull]
        public virtual string ItemType { get; protected set; }

        public virtual string ItemKey { get; protected set; }

        public virtual decimal OriginalPaymentAmount { get; protected set; }

        public virtual decimal PaymentDiscount { get; protected set; }

        public virtual decimal ActualPaymentAmount { get; protected set; }

        public virtual decimal RefundAmount { get; protected set; }

        public virtual decimal PendingRefundAmount { get; protected set; }

        public ExtraPropertyDictionary ExtraProperties { get; protected set; }

        #endregion

        public virtual Guid StoreId { get; protected set; }

        public void SetStoreId(Guid storeId)
        {
            StoreId = storeId;
        }

        protected PaymentItem()
        {
            ExtraProperties = new ExtraPropertyDictionary();
            this.SetDefaultsForExtraProperties(ProxyHelper.UnProxy(this).GetType());
        }
    }
}