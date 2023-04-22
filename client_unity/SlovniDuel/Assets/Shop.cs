using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class Shop : MonoBehaviour, IStoreListener
{
    private IStoreController controller;
    private IExtensionProvider extensions;
    
	public GameObject MessagePanel;
	public GameObject LoadingPanel;
    public GameConnection gameConnection;
    
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        //Debug.Log("inicializace obchodu fail");
        MessagePanel.GetComponent<MessagePanel>().ShowMessage(MessageType.ERROR, "error" + error, "fail" + error, true);
        
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        //Debug.Log("nakup fail");
        if(p != PurchaseFailureReason.UserCancelled)
            MessagePanel.GetComponent<MessagePanel>().ShowMessage(MessageType.ERROR, "Chyba", "Při nákupu doško k potížím, zkuste to prosím znovu" + p, true);
}

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
    	var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
        AppleTangle.Data(), /*Application.bundleIdentifier*/ "com.hiddenchickengames.slovniduel");
        
    	var result = validator.Validate(args.purchasedProduct.receipt);

        controller.ConfirmPendingPurchase(args.purchasedProduct);
        string purchaseToken = "TEST";

        //args.purchasedProduct.definition.id
        //gameConnection.PurchaseSuccessFull("GOOGLE", args.purchasedProduct.definition.id, args.purchasedProduct.transactionID, purchaseToken);


        foreach (IPurchaseReceipt productReceipt in result) {
			Debug.Log("UNITYYY:" + productReceipt.productID);
			Debug.Log(productReceipt.purchaseDate);
			Debug.Log(productReceipt.transactionID);

            string productID = productReceipt.productID;
            string transactionID = productReceipt.transactionID;


            GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
			if (null != google) {
				// This is Google's Order ID.
				// Note that it is null when testing in the sandbox
				// because Google's sandbox does not provide Order IDs.
				Debug.Log(google.transactionID);
				Debug.Log(google.purchaseState);
				Debug.Log(google.purchaseToken);

                purchaseToken = google.purchaseToken;
            }


            gameConnection.PurchaseSuccessFull("GOOGLE_PLAY", productReceipt.productID, productReceipt.transactionID, purchaseToken);
            gameConnection.UpdateProfile();

            AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
			if (null != apple) {
				Debug.Log(apple.originalTransactionIdentifier);
				Debug.Log(apple.subscriptionExpirationDate);
				Debug.Log(apple.cancellationDate);
				Debug.Log(apple.quantity);
			}
		}    
        
        return PurchaseProcessingResult.Complete;
    }

	bool IsInitialized()
    {
        return controller != null && extensions != null;
    }
    
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;
        
        LoadingPanel.SetActive(false);


        Product product = controller.products.WithID("hiddenchicken.com.payments.10words");
        if (product != null)
        {
            controller.ConfirmPendingPurchase(product);
        }

        product = controller.products.WithID("hiddenchicken.com.payments.35words");
        if (product != null)
        {
            controller.ConfirmPendingPurchase(product);
        }

        product = controller.products.WithID("hiddenchicken.com.payments.100words");
        if (product != null)
        {
            controller.ConfirmPendingPurchase(product);
        }

        product = controller.products.WithID("hiddenchicken.com.payments.150words");
        if (product != null)
        {
            controller.ConfirmPendingPurchase(product);
        }
    }

    public void InitShop() {
        var module = StandardPurchasingModule.Instance();
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);

        builder.AddProduct("hiddenchicken.com.payments.10words", ProductType.Consumable);
        builder.AddProduct("hiddenchicken.com.payments.35words", ProductType.Consumable);
        builder.AddProduct("hiddenchicken.com.payments.100words", ProductType.Consumable);
        builder.AddProduct("hiddenchicken.com.payments.150words", ProductType.Consumable);
        UnityPurchasing.Initialize(this, builder);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Buy(string productId)
    {
    	if(IsInitialized()){
        	controller.InitiatePurchase(productId);
        }else{
        	OnPurchaseFailed(null, PurchaseFailureReason.Unknown);
        }
    }
}
