/*
 * Author: Tùng Lương
 * Create on: 05/09/2016
 * Description: Singleton
 */

public abstract class Singleton<T> where T : new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }
   
    public static void Reset(){
        instance = new T();
    }
  
}
