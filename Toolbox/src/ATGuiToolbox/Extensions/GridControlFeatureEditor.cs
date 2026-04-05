using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using System.Reflection;

namespace AT.Toolbox.Extensions
{
  /// <summary>
  /// Редактор для работы с расширителями XtraGrid в дизайнере
  /// </summary>
  public class GridControlFeatureEditor : CollectionEditor
  {
    private static Type[] featureTypes;
    private static object typesLock = new object();
    
    public GridControlFeatureEditor(Type type) : base(type) { }

    protected override Type[] CreateNewItemTypes()
    {
      if (featureTypes == null)
      {
        InitFeatureTypes();
      }
      return featureTypes;
    }

    /// <summary>
    /// Загружаем из всех сборок домена, помеченных атрибутом IsATToolbox,
    /// типы, унаследованные от GridControlFeature
    /// </summary>
    private static void InitFeatureTypes()
    {
      lock (typesLock)
      {
        if (featureTypes == null)
        {
          // В домене во время разработки может быть несколько копий одной сборки,
          // поэтому исключаем повторяющиеся имена
          List<string> typeNames = new List<string>(); 

          List<Type> types = new List<Type>();
          foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
          {
            if (!asm.IsAtToolbox())
              continue;

            Type[] type_list = null;

            try
            {
              type_list = asm.GetTypes();
            }
            catch 
            {
              continue;
            }

            foreach (Type currentType in type_list)
            {
              if (!currentType.IsAbstract
                && currentType.IsSubclassOf(typeof(GridControlFeature))
                && !typeNames.Contains(currentType.FullName))
              {
                types.Add(currentType);
                typeNames.Add(currentType.FullName);
              }
            }
          }
          featureTypes = types.ToArray();
        }
      }
    }
  }
}
