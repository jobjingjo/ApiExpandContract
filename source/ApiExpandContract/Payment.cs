using System;

namespace ApiExpandContract
{
    public record Payment
    {

    }

    public record CashPayment : Payment
    {
        public decimal Amount { get; set; }
    }

    public record CreditPayment : Payment
    {
        public decimal Amount { get; set; }
    }
}
