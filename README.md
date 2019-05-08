# ExternalImageIntentRecieverUnity

This allows you to recieve External Android image intent within unity.

E.g. Recieve an image when shared from app like amazon

To enable your App to appear in share list
Add intent-filters in AndroidManifest.xml file under Application tag
(<----
<intent-filter>
        <action android:name="android.intent.action.SEND" />
        <category android:name="android.intent.category.DEFAULT" />
        <data android:mimeType="image/*" />
</intent-filter>
<intent-filter>
        <action android:name="android.intent.action.SEND" />
        <category android:name="android.intent.category.DEFAULT" />
        <data android:mimeType="text/plain" />
</intent-filter>
<intent-filter>
        <action android:name="android.intent.action.SEND_MULTIPLE" />
        <category android:name="android.intent.category.DEFAULT" />
        <data android:mimeType="image/*" />
</intent-filter>
---->)


To Recieve the Image: 
  Add the ExternalIntentReciever class to scene:
    Add new Empty GameObject < Add ExternalIntentReciever Script to gameObject

Then From Any Script Subscribe to (onGetImage) Action in start:

  ExternalIntentReciever.OnGetImage += delegate(Texture2D tex) =>{
    // Whatever you need to do with yor Image
  };
 
