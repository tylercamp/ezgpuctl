
Each dockable pane is held in `GPUControl.MainPanes`. Changes which involve saving new settings will go from
the appropriate MainPane back to the MainWindow, which saves those changes and completely rebuilds the
`MainWindowViewModel`.

"MVVM" is "used" but not adhered to very well. The need to sync references by name, the need for
a "sticky observable collection" (where the Default policy/profile is always at the bottom), and the
"intrinsic" nature of the default model objects (ie, not stored explicitly in the model, but constructed
at runtime based on available GPUs) made it non-trivial to manage everything through MVVM.

There isn't a strict methodology for when MVVM was/wasn't applied. I used it where possible for convenience
with WPF, and broke from the pattern for those non-trivial cases. Most uses of `ObservableCollection` can
be replaced with `List` without breaking anything.

For things like the MainWindow where no model-based view-model was appropriate, I added a control-specific
view-model definition above the view class.

Logic dealing with GPU data fetching and OC application is held in the `GPUControl.Lib` project.

Modal windows and sub-controls are in this project under `Controls`.

I used the MVVM utilities from Microsoft Community Toolkit to reduce boilerplate. Namely, extending
from `ObservableObject` for implementing `INotifyPropertyChanged`, decorating private fields with
`[ObservableProperty]` to auto-gen the appropriate getters/setters, and similar `[NotifyPropertyChangedFor]`
and `NotifyCanExecuteChangedFor` to further propagate changes to other properties.

I used AvalonDock to allow hiding/rearranging views since I'm not a UI designer and have no clue what
a good layout would look like for this.