namespace MyKhronus.WPF.UIModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;

public abstract class ObservableViewModelCollection<TModel, TViewModel> : ObservableCollection<TViewModel>
{

    public abstract void LoadItems(IEnumerable<TModel> models);

    public abstract void Add(TModel model);

    public abstract void Remove(TModel model);

}
