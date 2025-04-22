using System;
using System.Collections.Generic;

namespace Payments.Domain.ApiModel;

public class PagamentoWebHookAsaasRequest
{
    public string Event { get; set; }
    public PaymentDetails? Payment { get; set; }
}

public class PaymentDetails
{
    public string Object { get; set; }
    public string Id { get; set; }
    public DateTime? DateCreated { get; set; }
    public string Customer { get; set; }
    public string Subscription { get; set; }
    public string Installment { get; set; }
    public string PaymentLink { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? OriginalDueDate { get; set; }
    public float? Value { get; set; }
    public float? NetValue { get; set; }
    public float? OriginalValue { get; set; }
    public float? InterestValue { get; set; }
    public string Description { get; set; }
    public string ExternalReference { get; set; }
    public string BillingType { get; set; }
    public string Status { get; set; }
    public string PixTransaction { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? ClientPaymentDate { get; set; }
    public string InstallmentNumber { get; set; }
    public DateTime? CreditDate { get; set; }
    public DateTime? EstimatedCreditDate { get; set; }
    public string InvoiceUrl { get; set; }
    public string BankSlipUrl { get; set; }
    public string TransactionReceiptUrl { get; set; }
    public string InvoiceNumber { get; set; }
    public bool? Deleted { get; set; }
    public bool? Anticipated { get; set; }
    public bool? Anticipable { get; set; }
    public string LastInvoiceViewedDate { get; set; }
    public string LastBankSlipViewedDate { get; set; }
    public bool? PostalService { get; set; }
    public CreditCard? CreditCard { get; set; }
    public Discount? Discount { get; set; }
    public Fine? Fine { get; set; }
    public Interest? Interest { get; set; }
    public Split[]? Split { get; set; }
    public Chargeback? Chargeback { get; set; }
    public object? Refunds { get; set; }
}

public class CreditCard
{
    public string CreditCardNumber { get; set; }
    public string CreditCardBrand { get; set; }
    public string CreditCardToken { get; set; }
}

public class Split
{
    public string WalletId { get; set; }
    public decimal? FixedValue { get; set; }
    public decimal? PercentualValue { get; set; }
    public string Status { get; set; }
    public object? RefusalReason { get; set; }
}


public class Interest
{
    public decimal? Value { get; set; }
    public string Type { get; set; }
}

public class Fine
{
    public decimal? Value { get; set; }
    public string Type { get; set; }
}

public class Discount
{
    public decimal? Value { get; set; }
    public int? DueDateLimitDays { get; set; }
    public string Type { get; set; }
}

public class Chargeback
{
    public string Status { get; set; }
    public string Reason { get; set; }
}

public class AuthOptions
{
    public string ApiKey { get; set; }
    public List<string> AuthHeaderProviders { get; set; } = new();
}