using Smartwyre.DeveloperTest.Extensions;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using System;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    private static readonly string _rebateCmdFlag = "-rebate";
    private static readonly string _productCmdFlag = "-product";
    private static readonly string _volumeCmdFlag = "-volume";

    private static readonly decimal DEFAULT_VOLUME = -1m;

    static void Main(string[] args)
    {
        var Service = new RebateService();
        (string rebateId, string productId, decimal volume) = ParseCommandLine(args);

        char? UserResponse;
        do
        {
            GetUserInput(ref rebateId, ref productId, ref volume);
            CalculateRebateRequest request = new CalculateRebateRequest()
            {
                RebateIdentifier = rebateId,
                ProductIdentifier = productId,
                Volume = volume
            };

            var Result = Service.Calculate(request);
            Console.WriteLine(Result.ToDisplayString());
            Console.WriteLine("\nTo enter more values, press 'c', or any other key to exit ");
            UserResponse = Console.ReadKey().KeyChar;
            Console.WriteLine("\n");
            InitializeValues(out rebateId, out productId, out volume);
        }
        while (UserResponse == 'c');

    }

    private static void InitializeValues(out string rebateId, out string productId, out decimal volume)
    {
        rebateId = null;
        productId = null;
        volume = DEFAULT_VOLUME;
    }

    private static void GetUserInput(ref string rebateId, ref string productId, ref decimal volume)
    {
        if (rebateId == null)
        {
            Console.WriteLine("Please enter a rebate ID: ");
            rebateId = Console.ReadLine();
        }

        if (productId == null)
        {
            Console.WriteLine("Please enter a product ID: ");
            productId = Console.ReadLine();
        }

        while (volume == DEFAULT_VOLUME)
        {
            Console.WriteLine("Please enter a volume greater than zero: ");
            var VolumeInput = Console.ReadLine();
            if (Decimal.TryParse(VolumeInput, out decimal TempVolume))
            {
                volume = TempVolume > 0 ? TempVolume : DEFAULT_VOLUME;
            }
        }
    }

    private static (string, string, decimal) ParseCommandLine(string[] args)
    {
        // TODO: refactor this method
        string rebateId = null;
        string productId = null;
        string volumeInput = null;
        decimal volume = DEFAULT_VOLUME;

        if (args.Length > 0)
        {
            bool rebateArgFound = false;
            bool productArgFound = false;
            bool volumeArgFound = false;
            foreach (string arg in args)
            {
                if(rebateArgFound
                    && rebateId == null
                    && !arg.Equals(_rebateCmdFlag)
                    && !arg.Equals(_productCmdFlag)
                    && !arg.Equals(_volumeCmdFlag))
                {
                    rebateId = arg;
                }

                if (productArgFound
                    && productId == null
                    && !arg.Equals(_rebateCmdFlag)
                    && !arg.Equals(_productCmdFlag)
                    && !arg.Equals(_volumeCmdFlag))
                {
                    productId = arg;
                }

                if (volumeArgFound
                    && volume == DEFAULT_VOLUME
                    && !arg.Equals(_rebateCmdFlag)
                    && !arg.Equals(_productCmdFlag)
                    && !arg.Equals(_volumeCmdFlag))
                {
                    volumeInput = arg;
                    if(Decimal.TryParse(volumeInput, out decimal TempVolume))
                    {
                        volume  = TempVolume > 0 ? TempVolume : DEFAULT_VOLUME;
                    }
                }

                rebateArgFound = arg.Equals(_rebateCmdFlag);
                productArgFound = arg.Equals(_productCmdFlag);
                volumeArgFound = arg.Equals(_volumeCmdFlag);

                if (rebateId != null && productId != null && volumeInput != null) break;
            }
        }

        return (rebateId, productId, volume);
    }
}
