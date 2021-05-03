# ModelFramework
ModelFramework is constituted with several suggestions on how to separate concerns about POCOs from UI design patterns like MVVM.   

## Unified method of accessing models

>Before speaking her name,  
 She had been nothing but a gesture,  
 When I spoke her name,  
 She came to me and became a flower.  
 ...   
 *\- Kim Chun-Soo, Flower*. posted [here](https://blog.naver.com/elguapo81/20170048417)
 
A model which has POCOs can be accessed by it's URI.  
You just call model's name, then framework provides it!  
Framework is so generous that it'll always respond to your call, although any of you have detached ones.  

```c#
Uri Name = new Uri("hello://here/?greeting");
HelloModel model = Inventory.Get<HelloModel>(Name);
```

Under the cover, the framework
1. Instantiates a provider of a type which corresponds to "hello" scheme.
2. Sets the provider's URI with "hello://here/" and open it.
3. Asks the provider to create a HelloModel whose URI is "hello://here/?greeting".
4. Puts the model into Inventory and load its contents.

If you request same model next time, then you will get same object from Inventory directly.

## Encapsulation of providers
