using System.Collections.Generic;

namespace Unity.Services.Analytics
{
    public class StandardEventSample
    {
        public static void RecordMinimalAdImpressionEvent()
        {
            AdImpressionEvent adImpression = new AdImpressionEvent();
            adImpression.AdCompletionStatus = AdCompletionStatus.Completed;
            adImpression.AdProvider = AdProvider.UnityAds;
            adImpression.PlacementName = "PLACEMENTNAME";
            adImpression.PlacementId = "PLACEMENTID";

            AnalyticsService.Instance.RecordEvent(adImpression);
        }

        public static void RecordCompleteAdImpressionEvent()
        {
            AdImpressionEvent adImpression = new AdImpressionEvent();
            adImpression.AdCompletionStatus = AdCompletionStatus.Completed;
            adImpression.AdProvider = AdProvider.UnityAds;
            adImpression.PlacementName = "PLACEMENTNAME";
            adImpression.PlacementId = "PLACEMENTID";
            adImpression.PlacementType = AdPlacementType.BANNER;
            adImpression.AdEcpmUsd = 123.4;
            adImpression.AdSdkVersion = "1.2.3";
            adImpression.AdImpressionId = "IMPRESSIVE";
            adImpression.AdStoreDestinationId = "DSTID";
            adImpression.AdMediaType = "MOVIE";
            adImpression.AdTimeWatchedMs = 1234;
            adImpression.AdTimeCloseButtonShownMs = 5678;
            adImpression.AdLengthMs = 2345;
            adImpression.AdHasClicked = false;
            adImpression.AdSource = "ADSRC";
            adImpression.AdStatusCallback = "STATCALL";

            AnalyticsService.Instance.RecordEvent(adImpression);
        }

        public static void RecordSaleTransactionWithOnlyRequiredValues()
        {
            TransactionEvent transaction = new TransactionEvent();
            transaction.TransactionName = "emptySale";
            transaction.TransactionType = TransactionType.SALE;

            AnalyticsService.Instance.RecordEvent(transaction);
        }

        public static void RecordSaleTransactionWithRealCurrency()
        {
            TransactionEvent transaction = new TransactionEvent();
            transaction.TransactionName = "sellItem";
            transaction.TransactionType = TransactionType.SALE;
            transaction.SpentItems.Add(new TransactionItem
            {
                ItemName = "thePickOfDestiny",
                ItemAmount = 1,
                ItemType = "collectable"
            });
            transaction.ReceivedRealCurrency = new TransactionRealCurrency
            {
                RealCurrencyType = "EUR",
                RealCurrencyAmount = AnalyticsService.Instance.ConvertCurrencyToMinorUnits("EUR", 3.99)
            };

            AnalyticsService.Instance.RecordEvent(transaction);
        }

        public static void RecordSaleTransactionWithVirtualCurrency()
        {
            TransactionEvent transaction = new TransactionEvent();
            transaction.TransactionName = "sellItem";
            transaction.TransactionType = TransactionType.SALE;
            transaction.SpentItems.Add(new TransactionItem
            {
                ItemName = "elucidator",
                ItemAmount = 1,
                ItemType = "sword"
            });
            transaction.ReceivedVirtualCurrencies.Add(new TransactionVirtualCurrency
            {
                VirtualCurrencyType = VirtualCurrencyType.GRIND,
                VirtualCurrencyAmount = 125000,
                VirtualCurrencyName = "Cor"
            });

            AnalyticsService.Instance.RecordEvent(transaction);
        }

        public static void RecordSaleTransactionWithMultipleVirtualCurrencies()
        {
            TransactionEvent transaction = new TransactionEvent();
            transaction.TransactionName = "sellItem";
            transaction.TransactionType = TransactionType.SALE;
            transaction.SpentItems.Add(new TransactionItem
            {
                ItemName = "darkRepulser",
                ItemAmount = 1,
                ItemType = "weapon"
            });
            transaction.ReceivedVirtualCurrencies.Add(new TransactionVirtualCurrency
            {
                VirtualCurrencyType = VirtualCurrencyType.PREMIUM,
                VirtualCurrencyAmount = 100,
                VirtualCurrencyName = "Soul Points"
            });
            transaction.ReceivedVirtualCurrencies.Add(new TransactionVirtualCurrency
            {
                VirtualCurrencyType = VirtualCurrencyType.GRIND,
                VirtualCurrencyAmount = 50000,
                VirtualCurrencyName = "Gold Coins"
            });

            AnalyticsService.Instance.RecordEvent(transaction);
        }

        public static void RecordTradeEventWithOneItem()
        {
            TransactionEvent transaction = new TransactionEvent();
            transaction.TransactionName = "tradeItems";
            transaction.TransactionType = TransactionType.TRADE;
            transaction.SpentItems.Add(new TransactionItem
            {
                ItemName = "marketStall",
                ItemAmount = 1,
                ItemType = "special"
            });
            transaction.ReceivedItems.Add(new TransactionItem
            {
                ItemName = "cabbage",
                ItemAmount = 50,
                ItemType = "food"
            });

            AnalyticsService.Instance.RecordEvent(transaction);
        }

        public static void RecordTradeEventWithMultipleItems()
        {
            TransactionEvent transaction = new TransactionEvent();
            transaction.TransactionName = "tradeItems";
            transaction.TransactionType = TransactionType.TRADE;
            transaction.SpentItems.Add(new TransactionItem
            {
                ItemName = "flour",
                ItemAmount = 100,
                ItemType = "food",
            });
            transaction.SpentItems.Add(new TransactionItem
            {
                ItemName = "egg",
                ItemAmount = 1,
                ItemType = "food",
            });
            transaction.SpentItems.Add(new TransactionItem
            {
                ItemName = "milk",
                ItemAmount = 200,
                ItemType = "food",
            });
            transaction.SpentItems.Add(new TransactionItem
            {
                ItemName = "salt",
                ItemAmount = 1,
                ItemType = "food",
            });
            transaction.SpentItems.Add(new TransactionItem
            {
                ItemName = "heavyCream",
                ItemAmount = 150,
                ItemType = "food",
            });
            transaction.SpentItems.Add(new TransactionItem
            {
                ItemName = "sugar",
                ItemAmount = 15,
                ItemType = "food",
            });
            transaction.ReceivedItems.Add(new TransactionItem
            {
                ItemName = "pancake",
                ItemAmount = 2,
                ItemType = "food",
            });
            transaction.ReceivedItems.Add(new TransactionItem
            {
                ItemName = "whippedCream",
                ItemAmount = 165,
                ItemType = "food",
            });

            AnalyticsService.Instance.RecordEvent(transaction);
        }

        public static void RecordSaleEventWithOptionalParameters()
        {
            TransactionEvent transaction = new TransactionEvent();

            transaction.PaymentCountry = "PL";
            transaction.ProductId = "productid987";
            transaction.TransactionId = "0118-999-881-999-119-725-3";
            transaction.TransactionReceipt = "transactionreceipt";
            transaction.TransactionReceiptSignature = "signature";
            transaction.TransactionServer = TransactionServer.APPLE;
            transaction.TransactorID = "transactorid-0118-999-881-999-119-725-3";
            transaction.StoreItemSkuId = "storeitemskuid";
            transaction.StoreItemId = "storeitemid";
            transaction.StoreId = "storeid";
            transaction.StoreSourceId = "storesourceid";
            transaction.TransactionName = "transactionName";
            transaction.TransactionType = TransactionType.SALE;

            AnalyticsService.Instance.RecordEvent(transaction);
        }

        public static void RecordAcquisitionSourceEventWithOnlyRequiredValues()
        {
            AcquisitionSourceEvent acquisitionSource = new AcquisitionSourceEvent();

            acquisitionSource.AcquisitionChannel = "CHNL";
            acquisitionSource.AcquisitionCampaignId = "123-456-efg";
            acquisitionSource.AcquisitionCreativeId = "cre-ati-vei-d";
            acquisitionSource.AcquisitionCampaignName = "Interstitial:Halloween21";
            acquisitionSource.AcquisitionProvider = "AppsFlyer";

            AnalyticsService.Instance.RecordEvent(acquisitionSource);
        }

        public static void RecordAcquisitionSourceEventWithOptionalParameters()
        {
            AcquisitionSourceEvent acquisitionSource = new AcquisitionSourceEvent();

            acquisitionSource.AcquisitionChannel = "CHNL";
            acquisitionSource.AcquisitionCampaignId = "123-456-efg";
            acquisitionSource.AcquisitionCreativeId = "cre-ati-vei-d";
            acquisitionSource.AcquisitionCampaignName = "Interstitial:Halloween21";
            acquisitionSource.AcquisitionProvider = "AppsFlyer";
            acquisitionSource.AcquisitionCampaignType = "CPI";
            acquisitionSource.AcquisitionCost = 123.4f;
            acquisitionSource.AcquisitionCostCurrency = "BGN";
            acquisitionSource.AcquisitionNetwork = "Ironsource";

            AnalyticsService.Instance.RecordEvent(acquisitionSource);
        }

        public static void RecordPurchaseEventWithOneItem()
        {
            TransactionEvent transaction = new TransactionEvent();
            transaction.TransactionName = "itemPurchase";
            transaction.TransactionType = TransactionType.PURCHASE;
            transaction.SpentRealCurrency = new TransactionRealCurrency
            {
                RealCurrencyAmount = AnalyticsService.Instance.ConvertCurrencyToMinorUnits("JPY", 39800),
                RealCurrencyType = "JPY"
            };
            transaction.ReceivedItems.Add(new TransactionItem
            {
                ItemName = "nerveGear",
                ItemAmount = 1,
                ItemType = "electronics",
            });

            AnalyticsService.Instance.RecordEvent(transaction);
        }

        public static void RecordPurchaseEventWithMultipleItems()
        {
            TransactionEvent transaction = new TransactionEvent();
            transaction.TransactionName = "itemPurchase";
            transaction.TransactionType = TransactionType.PURCHASE;
            transaction.SpentVirtualCurrencies.Add(new TransactionVirtualCurrency
            {
                VirtualCurrencyType = VirtualCurrencyType.GRIND,
                VirtualCurrencyAmount = 200500,
                VirtualCurrencyName = "Pokemon Dollar"
            });
            transaction.ReceivedItems.Add(new TransactionItem
            {
                ItemName = "magicarp",
                ItemAmount = 1,
                ItemType = "pokemon",
            });
            transaction.ReceivedItems.Add(new TransactionItem
            {
                ItemName = "rareCandy",
                ItemAmount = 20,
                ItemType = "item",
            });

            AnalyticsService.Instance.RecordEvent(transaction);
        }

        public static void RecordPurchaseEventWithMultipleCurrencies()
        {
            TransactionEvent transaction = new TransactionEvent();
            transaction.TransactionName = "itemPurchase";
            transaction.TransactionType = TransactionType.PURCHASE;
            transaction.ReceivedItems.Add(new TransactionItem
            {
                ItemName = "holySwordExcalibur",
                ItemAmount = 1,
                ItemType = "weapon"
            });
            transaction.SpentVirtualCurrencies.Add(new TransactionVirtualCurrency
            {
                VirtualCurrencyType = VirtualCurrencyType.GRIND,
                VirtualCurrencyAmount = 4000000,
                VirtualCurrencyName = "Cor"
            });
            transaction.SpentVirtualCurrencies.Add(new TransactionVirtualCurrency
            {
                VirtualCurrencyType = VirtualCurrencyType.PREMIUM,
                VirtualCurrencyAmount = 50000,
                VirtualCurrencyName = "Credit"
            });

            AnalyticsService.Instance.RecordEvent(transaction);
        }
    }
}
