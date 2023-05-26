using System;
using Xunit;
using Moq;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;
using System.Threading;
using Smartwyre.DeveloperTest.Services;

namespace Smartwyre.DeveloperTest.Tests
{

    public class PaymentServiceTests
    {
        private const string TEST_PRODUCT_ID = "productTestId";
        private const string TEST_REBATE_ID = "rebateTesstId";
        private const decimal ZERO_AMOUNT_OR_PERCENT = 0m;
        private const decimal NON_ZERO_AMOUNT_OR_PERCENT = 1.0m;
        private const decimal NON_ZERO_TEST_PERCENT = 0.1m;

        [Fact]
        public void NullRebate()
        {
            var ProductMock = new Mock<IProductDataStore>();
            ProductMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(new Product());

            var RebateMock = new Mock<IRebateDataStore>();
            RebateMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns<IRebateDataStore>(null);

            var testSubject = new RebateService(RebateMock.Object, ProductMock.Object);
            Assert.Throws<NullReferenceException>(() => testSubject.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = TEST_PRODUCT_ID,
                RebateIdentifier = TEST_REBATE_ID
            }));
        }

        [Fact]
        public void FixedRateRebateNullProduct()
        {
            var ProductMock = new Mock<IProductDataStore>();
            ProductMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns<IProductDataStore>(null);

            var RebateMock = new Mock<IRebateDataStore>();
            RebateMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(new Rebate { Incentive = IncentiveType.FixedRateRebate});

            var testSubject = new RebateService(RebateMock.Object, ProductMock.Object);
            var result = testSubject.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = TEST_PRODUCT_ID,
                RebateIdentifier = TEST_REBATE_ID
            });

            Assert.False(result.Success);
        }

        [Fact]
        public void AmountPerUomRebateNullProduct()
        {
            var ProductMock = new Mock<IProductDataStore>();
            ProductMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns<IProductDataStore>(null);

            var RebateMock = new Mock<IRebateDataStore>();
            RebateMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(new Rebate { Incentive = IncentiveType.AmountPerUom });

            var testSubject = new RebateService(RebateMock.Object, ProductMock.Object);
            var result = testSubject.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = TEST_PRODUCT_ID,
                RebateIdentifier = TEST_REBATE_ID
            });

            Assert.False(result.Success);
        }

        [Fact]
        public void FixedCashAmtRebateOnUnsupportedProduct()
        {
            var ProductMock = new Mock<IProductDataStore>();
            ProductMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(new Product() { SupportedIncentives = SupportedIncentiveType.AmountPerUom });

            var RebateMock = new Mock<IRebateDataStore>();
            RebateMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(new Rebate() { Incentive = IncentiveType.FixedCashAmount });

            var testSubject = new RebateService(RebateMock.Object, ProductMock.Object);
            var result = testSubject.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = TEST_PRODUCT_ID,
                RebateIdentifier = TEST_REBATE_ID
            });

            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        [Fact]
        public void FixedRateRebateOnUnsupportedProduct()
        {
            var ProductMock = new Mock<IProductDataStore>();
            ProductMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(new Product() { SupportedIncentives = SupportedIncentiveType.AmountPerUom });

            var RebateMock = new Mock<IRebateDataStore>();
            RebateMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(new Rebate() { Incentive = IncentiveType.FixedRateRebate });

            var testSubject = new RebateService(RebateMock.Object, ProductMock.Object);
            var result = testSubject.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = TEST_PRODUCT_ID,
                RebateIdentifier = TEST_REBATE_ID
            });

            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        [Fact]
        public void AmountPerUomRebateOnUnsupportedProduct()
        {
            var ProductMock = new Mock<IProductDataStore>();
            ProductMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(new Product() { SupportedIncentives = SupportedIncentiveType.FixedRateRebate });

            var RebateMock = new Mock<IRebateDataStore>();
            RebateMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(new Rebate() { Incentive = IncentiveType.AmountPerUom });

            var testSubject = new RebateService(RebateMock.Object, ProductMock.Object);
            var result = testSubject.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = TEST_PRODUCT_ID,
                RebateIdentifier = TEST_REBATE_ID
            });

            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        [Fact]
        public void FixedCashAmtRebateWithZeroAmount()
        {
            var ProductMock = new Mock<IProductDataStore>();
            ProductMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(new Product() { SupportedIncentives = SupportedIncentiveType.FixedCashAmount });

            var RebateMock = new Mock<IRebateDataStore>();
            RebateMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(new Rebate() { Incentive = IncentiveType.FixedCashAmount, Amount = ZERO_AMOUNT_OR_PERCENT });

            var testSubject = new RebateService(RebateMock.Object, ProductMock.Object);
            var result = testSubject.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = TEST_PRODUCT_ID,
                RebateIdentifier = TEST_REBATE_ID
            });

            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        [Fact]
        public void AmountPerUomRebateWithZeroAmount()
        {
            var ProductMock = new Mock<IProductDataStore>();
            ProductMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(new Product() { SupportedIncentives = SupportedIncentiveType.AmountPerUom });

            var RebateMock = new Mock<IRebateDataStore>();
            RebateMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(new Rebate() { Incentive = IncentiveType.AmountPerUom, Amount = ZERO_AMOUNT_OR_PERCENT });

            var testSubject = new RebateService(RebateMock.Object, ProductMock.Object);
            var result = testSubject.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = TEST_PRODUCT_ID,
                RebateIdentifier = TEST_REBATE_ID,
                Volume = NON_ZERO_AMOUNT_OR_PERCENT
            });

            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        [Fact]
        public void FixedRateRebateWithZeroPercent()
        {
            var ProductMock = new Mock<IProductDataStore>();
            ProductMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(new Product() { SupportedIncentives = SupportedIncentiveType.FixedRateRebate });

            var RebateMock = new Mock<IRebateDataStore>();
            RebateMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(new Rebate() { Incentive = IncentiveType.FixedRateRebate, Percentage = ZERO_AMOUNT_OR_PERCENT });

            var testSubject = new RebateService(RebateMock.Object, ProductMock.Object);
            var result = testSubject.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = TEST_PRODUCT_ID,
                RebateIdentifier = TEST_REBATE_ID
            });

            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        [Fact]
        public void FixedRateRebateWithZeroPriceProduct()
        {
            var ProductMock = new Mock<IProductDataStore>();
            ProductMock.Setup(p => p.GetProduct(It.IsAny<string>()))
                .Returns(new Product() { SupportedIncentives = SupportedIncentiveType.FixedRateRebate, Price = ZERO_AMOUNT_OR_PERCENT });

            var RebateMock = new Mock<IRebateDataStore>();
            RebateMock.Setup(r => r.GetRebate(It.IsAny<string>()))
                .Returns(new Rebate() { Incentive = IncentiveType.FixedRateRebate, Percentage = NON_ZERO_AMOUNT_OR_PERCENT });

            var testSubject = new RebateService(RebateMock.Object, ProductMock.Object);
            var result = testSubject.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = TEST_PRODUCT_ID,
                RebateIdentifier = TEST_REBATE_ID
            });

            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        [Fact]
        public void FixedRateRebateWithZeroVolume()
        {
            var ProductMock = new Mock<IProductDataStore>();
            ProductMock.Setup(p => p.GetProduct(It.IsAny<string>()))
                .Returns(new Product() { SupportedIncentives = SupportedIncentiveType.FixedRateRebate, Price = NON_ZERO_AMOUNT_OR_PERCENT });

            var RebateMock = new Mock<IRebateDataStore>();
            RebateMock.Setup(r => r.GetRebate(It.IsAny<string>()))
                .Returns(new Rebate() { Incentive = IncentiveType.FixedRateRebate, Percentage = NON_ZERO_AMOUNT_OR_PERCENT });

            var testSubject = new RebateService(RebateMock.Object, ProductMock.Object);
            var result = testSubject.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = TEST_PRODUCT_ID,
                RebateIdentifier = TEST_REBATE_ID,
                Volume = ZERO_AMOUNT_OR_PERCENT
            });

            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        [Fact]
        public void AmountPerUomWithZeroVolume()
        {
            var ProductMock = new Mock<IProductDataStore>();
            ProductMock.Setup(p => p.GetProduct(It.IsAny<string>()))
                .Returns(new Product() { SupportedIncentives = SupportedIncentiveType.AmountPerUom });

            var RebateMock = new Mock<IRebateDataStore>();
            RebateMock.Setup(r => r.GetRebate(It.IsAny<string>()))
                .Returns(new Rebate() { Incentive = IncentiveType.AmountPerUom, Amount = NON_ZERO_AMOUNT_OR_PERCENT });

            var testSubject = new RebateService(RebateMock.Object, ProductMock.Object);
            var result = testSubject.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = TEST_PRODUCT_ID,
                RebateIdentifier = TEST_REBATE_ID,
                Volume = ZERO_AMOUNT_OR_PERCENT
            });

            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        [Fact]
        public void ValidFixedCashAmtRebate()
        {
            var ProductMock = new Mock<IProductDataStore>();
            ProductMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(new Product() { SupportedIncentives = SupportedIncentiveType.FixedCashAmount });

            var RebateMock = new Mock<IRebateDataStore>();
            var rebate = new Rebate() { Incentive = IncentiveType.FixedCashAmount, Amount = NON_ZERO_AMOUNT_OR_PERCENT };

            RebateMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(rebate);
            RebateMock.Setup(r => r.StoreCalculationResult(rebate, NON_ZERO_AMOUNT_OR_PERCENT)).Verifiable();

            var testSubject = new RebateService(RebateMock.Object, ProductMock.Object);
            var result = testSubject.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = TEST_PRODUCT_ID,
                RebateIdentifier = TEST_REBATE_ID
            });

            Assert.NotNull(result);
            Assert.True(result.Success);
            RebateMock.Verify();
        }

        [Fact]
        public void ValidFixedRateRebate()
        {
            var ProductMock = new Mock<IProductDataStore>();
            var product = new Product() { SupportedIncentives = SupportedIncentiveType.FixedRateRebate, Price = NON_ZERO_AMOUNT_OR_PERCENT };
            ProductMock.Setup(p => p.GetProduct(It.IsAny<string>()))
                .Returns(product);

            var RebateMock = new Mock<IRebateDataStore>();
            var rebate = new Rebate() { Incentive = IncentiveType.FixedRateRebate, Percentage = NON_ZERO_TEST_PERCENT };
            RebateMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(rebate);

            var request = new CalculateRebateRequest()
            {
                ProductIdentifier = TEST_PRODUCT_ID,
                RebateIdentifier = TEST_REBATE_ID,
                Volume = NON_ZERO_AMOUNT_OR_PERCENT
            };
            RebateMock.Setup(r => r.StoreCalculationResult(rebate, rebate.Percentage * product.Price * request.Volume)).Verifiable();

            var testSubject = new RebateService(RebateMock.Object, ProductMock.Object);
            var result = testSubject.Calculate(request);

            Assert.NotNull(result);
            Assert.True(result.Success);
            RebateMock.Verify();
        }

        [Fact]
        public void ValidAmountPerUomRebate()
        {
            var ProductMock = new Mock<IProductDataStore>();
            var product = new Product() { SupportedIncentives = SupportedIncentiveType.AmountPerUom };
            ProductMock.Setup(p => p.GetProduct(It.IsAny<string>()))
                .Returns(product);

            var RebateMock = new Mock<IRebateDataStore>();
            var rebate = new Rebate() { Incentive = IncentiveType.AmountPerUom, Amount = NON_ZERO_AMOUNT_OR_PERCENT };
            RebateMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(rebate);

            var request = new CalculateRebateRequest()
            {
                ProductIdentifier = TEST_PRODUCT_ID,
                RebateIdentifier = TEST_REBATE_ID,
                Volume = NON_ZERO_AMOUNT_OR_PERCENT
            };
            RebateMock.Setup(r => r.StoreCalculationResult(rebate, rebate.Amount * request.Volume)).Verifiable();

            var testSubject = new RebateService(RebateMock.Object, ProductMock.Object);
            var result = testSubject.Calculate(request);

            Assert.NotNull(result);
            Assert.True(result.Success);
            RebateMock.Verify();
        }

    }
}