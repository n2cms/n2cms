using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.XPath;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Persistence;
using N2.Security;

namespace N2.Xml
{
    public class N2XmlReader
    {
        #region Constructor

        public N2XmlReader(IEngine engine)
        {
            this.engine = engine;
        }

        #endregion

        #region Fields

        private readonly Engine.Logger<N2XmlReader> logger;
        private IEngine engine;
        private bool keepItemID = false;
        private bool useDiscriminator = true;

        #endregion

        #region Properties

        /// <summary>Gets or sets whether read items original ID value should be kept.</summary>
        public bool KeepItemID
        {
            get { return keepItemID; }
            set { keepItemID = value; }
        }

        /// <summary>Gets or sets whether the discriminator should be used to find types rather than type name.</summary>
        public bool UseDiscriminator
        {
            get { return useDiscriminator; }
            set { useDiscriminator = value; }
        }

        ///// <summary>Gets filters applied before recursing into next level.</summary>
        //public IList<N2.Collections.ItemFilter> PreFilters
        //{
        //    get { return preFilters; }
        //}

        #endregion

        public ContentItem ReadXml(string xml)
        {
            return Read(new StringReader(xml));
        }

        public ContentItem Read(TextReader input)
        {
            XPathDocument xpd = new XPathDocument(input);
            return ReadDocument(xpd);
        }

        public ContentItem Read(Stream input)
        {
            XPathDocument xpd = new XPathDocument(input);
            return ReadDocument(xpd);
        }

        private ContentItem ReadDocument(XPathDocument xpd)
        {
            XPathNavigator navigator = xpd.CreateNavigator();
            OnMovingToRootItem(navigator);

            return OnReadingItem(navigator);
        }

        protected virtual void OnMovingToRootItem(XPathNavigator navigator)
        {
            navigator.MoveToRoot();
            if (!navigator.MoveToFirstChild())
                throw new InvalidXmlException("Expected node n2 not found");
            if (!navigator.MoveToFirstChild())
                throw new InvalidXmlException("Expected node item not found");
        }

        protected virtual ContentItem OnReadingItem(XPathNavigator navigator)
        {
            Dictionary<string, string> attributes = GetAttributes(navigator);

            ItemDefinition definition = FindDefinition(attributes);
            ContentItem item = engine.Resolve<ContentActivator>().CreateInstance(definition.ItemType, null);

            OnSettingDefaultAttributes(attributes, item);

            //authorizedRoles
            navigator.MoveToFirstChild();
            OnReadingAuthorizedRoles(navigator, item);

            //details
            navigator.MoveToNext();
            OnReadingDetails(navigator, item);

            //detailCollections
            navigator.MoveToNext();
            OnReadingDetailCollections(navigator, item);

            //children
            navigator.MoveToNext();
            OnReadingChildren(navigator, item);

            navigator.MoveToParent();
            return item;
        }

        protected virtual ItemDefinition FindDefinition(Dictionary<string, string> attributes)
        {
            if (!UseDiscriminator)
            {
                string typeName = attributes["typeName"];
                Type t = Type.GetType(typeName);
                ItemDefinition d = engine.Definitions.GetDefinition(t);
                if (d == null)
                    throw new DefinitionNotFoundException("No definition found for type: " + typeName);
                return d;
            }
            else
            {
                string discriminator = attributes["discriminator"];
                foreach (ItemDefinition d in engine.Definitions.GetDefinitions())
                    if (d.Discriminator == discriminator)
                        return d;
                throw new DefinitionNotFoundException("No definition found for discriminator: " + discriminator);
            }
        }

        protected virtual void OnSettingDefaultAttributes(Dictionary<string, string> attributes, ContentItem item)
        {
            item.Created = Convert.ToDateTime(attributes["created"]);
            if (!string.IsNullOrEmpty(attributes["expires"]))
                item.Expires = Convert.ToDateTime(attributes["expires"]);
            if (KeepItemID)
                item.ID = Convert.ToInt32(attributes["id"]);
            item.Name = attributes["name"];
            item.Published = Convert.ToDateTime(attributes["published"]);
            item.SavedBy = attributes["savedBy"];
            item.SortOrder = Convert.ToInt32(attributes["sortOrder"]);
            item.Title = attributes["title"];
            item.Updated = Convert.ToDateTime(attributes["updated"]);
            item.Visible = Convert.ToBoolean(attributes["visible"]);
            if (!string.IsNullOrEmpty(attributes["zoneName"]))
                item.ZoneName = attributes["zoneName"];
        }

        private static Dictionary<string, string> GetAttributes(XPathNavigator navigator)
        {
            if (!navigator.MoveToFirstAttribute())
                throw new InvalidXmlException("node has no attributes: " + navigator.Name);
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            do
            {
                attributes.Add(navigator.Name, navigator.Value);
            } while (navigator.MoveToNextAttribute());
            navigator.MoveToParent();
            return attributes;
        }

        #region OnReadingAuthorizedRoles

        protected virtual void OnReadingAuthorizedRoles(XPathNavigator navigator, ContentItem item)
        {
            if (navigator.MoveToFirstChild())
            {
                do
                {
                    logger.Debug(navigator.Value);
                    item.AuthorizedRoles.Add(new AuthorizedRole(item, navigator.Value));
                } while (navigator.MoveToNext());
                navigator.MoveToParent();
            }
        }

        #endregion

        #region OnReadingDetails

        protected virtual void OnReadingDetails(XPathNavigator navigator, ContentItem item)
        {
            if (navigator.MoveToFirstChild())
            {
                do
                {
                    OnAddingDetail(navigator, item);
                } while (navigator.MoveToNext());
                navigator.MoveToParent();
            }
        }

        protected virtual void OnAddingDetail(XPathNavigator navigator, ContentItem item)
        {
            Dictionary<string, string> attributes = GetAttributes(navigator);
            string name = attributes["name"];
            Type type = Utility.TypeFromName(attributes["typeName"]);
            if (type != typeof (ContentItem))
                item[name] = ParseValue(navigator.Value, type);
            else
                logger.Debug("OnAddingDetail: Ignoring link detail."); //TODO resolve links
        }

        protected virtual object ParseValue(string xmlValue, Type type)
        {
            if (type == typeof (object))
            {
                byte[] buffer = Convert.FromBase64String(xmlValue);
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(new MemoryStream(buffer));
            }
            else
                return Utility.Convert(xmlValue, type);
        }

        #endregion

        #region OnReadingDetailCollections

        protected virtual void OnReadingDetailCollections(XPathNavigator navigator, ContentItem item)
        {
            if (navigator.MoveToFirstChild())
            {
                do
                {
                    OnReadingDetailsInCollection(navigator, item);
                } while (navigator.MoveToNext());
                navigator.MoveToParent();
            }
        }

        protected virtual void OnReadingDetailsInCollection(XPathNavigator navigator, ContentItem item)
        {
            if (navigator.MoveToFirstChild())
            {
                Dictionary<string, string> attributes = GetAttributes(navigator);
                DetailCollection collection = item.GetDetailCollection(attributes["name"], true);
                do
                {
                    OnAddingDetail(navigator, collection);
                } while (navigator.MoveToNext());
                navigator.MoveToParent();
            }
        }

        protected virtual void OnAddingDetail(XPathNavigator navigator, DetailCollection collection)
        {
            Dictionary<string, string> attributes = GetAttributes(navigator);
            string name = attributes["name"];
            Type type = Utility.TypeFromName(attributes["typeName"]);
            if (type != typeof (ContentItem))
                collection.Add(ParseValue(navigator.Value, type));
            else
                logger.Debug("OnAddingDetail: Ignoring link detail"); //TODO resolve links
        }

        #endregion

        #region OnReadingChildren

        private void OnReadingChildren(XPathNavigator navigator, ContentItem item)
        {
            if (navigator.MoveToFirstChild())
            {
                do
                {
                    ContentItem child = OnReadingItem(navigator);
                    child.AddTo(item);
                } while (navigator.MoveToNext());
                navigator.MoveToParent();
            }
        }

        #endregion
    }
}
