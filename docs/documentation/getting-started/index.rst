===============
Getting Started
===============

*N2CMS is a lightweight CMS framework* With just a few strokes of your keyboard a wonderful strongly typed 
model emerges complete with a management UI. You can spend the rest of your day building the best site imaginable.

*It's so .NET!*  With N2CMS, you build the model of the data that needs to be managed using C# or VB code in Visual Studio. The type below is picked up by the N2 engine at runtime and made available to be edited. The code uses an open API with multiple built-in options and unlimited customization options.

All you have to do is design your model class (inherit N2.ContentItem) and define which properties are editable by adding attributes

.. code-block:: c#

    [PageDefinition(TemplateUrl = "~/my/pageitemtemplate.aspx")]
    public class PageItem : N2.ContentItem
    {
        [EditableFreeTextArea]
        public virtual string Text { get; set; }
    }

See also: Editors via Attributes

.. toctree::
   :maxdepth: 3

   server-requirements.rst
