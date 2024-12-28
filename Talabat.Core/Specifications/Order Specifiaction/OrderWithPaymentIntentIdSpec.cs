using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregation;

namespace Talabat.Core.Specifications.Order_Specifiaction
{
	public class OrderWithPaymentIntentIdSpec :BaseSpecifications<Order>
	{
        public OrderWithPaymentIntentIdSpec(string PaymentIntentId):base(O=>O.PaymentIentId == PaymentIntentId)
        {
            
        }
    }
}
