# easyUpdater
.Net Updater Library details
You will need following code uploaded to your webhosting:
```
{
   "easyUpdate":{
      "update":[
         {
            "appID":"Example",
            "version":"2.0.0.0",
            "files":{
               "file":[
                  {
                     "url":"http://192.168.0.1/Example.exe",
                     "fileName":"Example.exe",
                     "md5":"7f6d718632a92873148c1acf156cb591"
                  }
               ],
               "file":[
                  {
                     "url":"http://192.168.0.1/easyUpdater.dll",
                     "fileName":"easyUpdater.dll",
                     "md5":"b752b5635194583b2dfd983b21567866"
                  }
               ]
            },
            "description":"Test message for Example appID"
         }
      ]
   }
}
```
Please copy following code and save it as data.json
Also make sure that you edit in 'Example' Project Form1.cs on line 51.
```
get { return new Uri("http://192.168.0.1/data.json"); }
```
