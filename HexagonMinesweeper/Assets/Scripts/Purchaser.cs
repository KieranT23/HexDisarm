using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class Purchaser : MonoBehaviour, IStoreListener
{
    public static Purchaser Instance;
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    private string noAdsProduct = "remove_ads";

    private bool isRestoringAds;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    /// <summary>
    /// This is called when the purchasing has been initialised
    /// </summary>
    /// <param name="controller">The store controller</param>
    /// <param name="extensions">The extension provider</param>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;

        //Restore all purchases
        RestorePurchases();
    }

    /// <summary>
    /// This is called when initalising the purchasing has failed
    /// </summary>
    /// <param name="error">The reason for the failure</param>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.LogError(string.Concat("OnInitializeFailed InitializationFailureReason:", error));
    }

    /// <summary>
    /// Process the purchase
    /// </summary>
    /// <param name="args">The purchase event arguments</param>
    /// <returns>Returns when complete</returns>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // A consumable product has been purchased by this user.
        if (String.Equals(args.purchasedProduct.definition.id, noAdsProduct, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS.Product: '{0}'", args.purchasedProduct.definition.id));
            AdManager.Instance.RemoveAds();
        }
       
        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// Purchasing failed
    /// </summary>
    /// <param name="product">The product that was failed to purchase</param>
    /// <param name="failureReason">The reason for the failure</param>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        if (failureReason == PurchaseFailureReason.DuplicateTransaction)
            AdManager.Instance.RemoveAds();
    }

    /// <summary>
    /// Initialise the purchasing
    /// </summary>
    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        //Consumable = can be purchased multiple times
        //Non-consumable = can only be purchased once
        //Subscription = recurring purchase
        builder.AddProduct(noAdsProduct, ProductType.NonConsumable);

        // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        // if the Product ID was configured differently between Apple and Google stores. Also note that
        // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
        // must only be referenced here. 

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>
    /// Buy a consumable product using its general identifier. Expect a response either through
    /// ProcessPurchase or OnPurchaseFailed asynchronously
    /// </summary>
    public void BuyNoAds()
    {
        BuyProductID(noAdsProduct);
    }

    /// <summary>
    /// Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    /// Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    /// </summary>
    public void RestorePurchases(bool showPopup = false)
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            IAppleExtensions apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                if (result && showPopup)
                {
                    PopupManager.Instance.ShowRestorePurchases();
                }
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log(string.Concat("RestorePurchases FAIL. Not supported on this platform. Current = ",
                Application.platform));
        }
    }

    /// <summary>
    /// Check if the purchasing is initialsied
    /// </summary>
    /// <returns></returns>
    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    /// <summary>
    /// Buy a product
    /// </summary>
    /// <param name="productId">The ID of the product to buy</param>
    private void BuyProductID(string productId)
    {
        Debug.Log("Buying product: " + productId);
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            Debug.Log("Has been initialised");
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.LogError("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.LogError("BuyProductID FAIL. Not initialized.");
        }  
    }
}
