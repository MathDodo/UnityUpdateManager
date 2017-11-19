using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public abstract class ResetablePoolComponent<T> : PoolableComponent where T : ResetablePoolComponent<T>, IResetableUnityPoolObject
{
    //The binding flags for this only used when IResetableUnityPoolObject is active
    private BindingFlags bindFlags;

    //This can hold all starting values of fields if the resetable interface is implemented
    private Dictionary<string, object> fieldValues;

    /// <summary>
    /// This is called through the IResetableUnityPoolObject interface and gets the field values if they are marked with the ResetField attribute, ONLY LET THE OBJECTPOOL CALL THIS METHOD.
    /// </summary>
    public virtual void ResetSetup()
    {
        fieldValues = new Dictionary<string, object>();

        bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        var fieldArray = GetType().GetFields(bindFlags).Where(f => f.IsDefined(typeof(ResetFieldAttribute), false)).ToArray();

        for (int i = 0; i < fieldArray.Length; i++)
        {
            fieldValues.Add(fieldArray[i].Name, fieldArray[i].GetValue(this));
        }
    }

    /// <summary>
    /// This method is called after Despawned and resets the fields markes with the ResetField attribute, ONLY LET THE OBJECTPOOL CALL THIS METHOD.
    /// </summary>
    public virtual void RunReset()
    {
        foreach (var pair in fieldValues)
        {
            GetType().GetField(pair.Key, bindFlags).SetValue(this, pair.Value);
        }
    }
}