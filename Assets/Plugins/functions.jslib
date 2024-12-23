mergeInto(LibraryManager.library, {
  OpenNewTabURL: function (url) {
    GoToURL(UTF8ToString(url))
  },
  RequestChangeInteractionValue: function (interactableID) {
    ClientRequestChangeInteractionValue(UTF8ToString(interactableID))
  },
});
