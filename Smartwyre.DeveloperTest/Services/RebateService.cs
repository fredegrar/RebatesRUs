using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;
using System;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private static IRebateDataStore _rebateDataStore;
    private static IProductDataStore _productDataStore;

    #region Constructors
    public RebateService()
    {
        //use default implementations
        _rebateDataStore = new RebateDataStore();
        _productDataStore = new ProductDataStore();
    }
    public RebateService(IRebateDataStore rebateDS, IProductDataStore productDS)
    {
        //if provided, use injected implementations
        _rebateDataStore = rebateDS ?? new RebateDataStore();
        _productDataStore = productDS ?? new ProductDataStore();
    }
    #endregion

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        Rebate rebate = _rebateDataStore.GetRebate(request.RebateIdentifier) ?? throw new NullReferenceException("rebate is null");
        Product product = _productDataStore.GetProduct(request.ProductIdentifier);

        var result = new CalculateRebateResult();

        var rebateAmount = 0m;

        switch (rebate.Incentive)
        {
            case IncentiveType.FixedCashAmount:
                (rebateAmount, result.Success) = EvaluateFixedCashRebate(rebate, product);
                break;

            case IncentiveType.FixedRateRebate:
                (rebateAmount, result.Success) = EvaluateFixedRateRebate(request, rebate, product);
                break;

            case IncentiveType.AmountPerUom:
                (rebateAmount, result.Success) = EvaluateAmountPerUomRebate(request, rebate, product);
                break;
        }

        if (result.Success)
        {
            _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);
        }

        return result;
    }

    #region private methods
    private static (decimal, bool) EvaluateAmountPerUomRebate(CalculateRebateRequest request, Rebate rebate, Product product)
    {
        var rebateAmount = 0m;
        var success = false;
        if (AmountPerUomREbateIsValid(request, rebate, product))
        {
            rebateAmount += rebate.Amount * request.Volume;
            success = true;
        }

        return (rebateAmount, success);
    }

    private static bool AmountPerUomREbateIsValid(CalculateRebateRequest request, Rebate rebate, Product product)
    {
        return product != null
                    && product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom)
                    && rebate.Amount != 0 && request.Volume != 0;
    }

    private static (decimal, bool) EvaluateFixedRateRebate(CalculateRebateRequest request, Rebate rebate, Product product)
    {
        var rebateAmount = 0m;
        var success = false;
        if (FixedRateRebateIsValid(request, rebate, product))
        {
            rebateAmount += product.Price * rebate.Percentage * request.Volume;
            success = true;
        }

        return (rebateAmount, success);
    }

    private static bool FixedRateRebateIsValid(CalculateRebateRequest request, Rebate rebate, Product product)
    {
        return product != null
                    && product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate)
                    && rebate.Percentage != 0 && product.Price != 0 && request.Volume != 0;
    }

    private static (decimal, bool) EvaluateFixedCashRebate(Rebate rebate, Product product)
    {
        var rebateAmount = 0m;
        var success = false;
        if (FixedCashRebateIsValid(rebate, product))
        {
            rebateAmount = rebate.Amount;
            success = true;
        }

        return (rebateAmount, success);
    }

    private static bool FixedCashRebateIsValid(Rebate rebate, Product product)
    {
        return product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount)
                    && rebate.Amount != 0;
    }
    #endregion
}
