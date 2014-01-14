using System.Diagnostics;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace N2.Tests.SchemaExporting
{
    [TestFixture]
    public class UpgradeTest
    {
        [Test, Ignore("Requires sql server")]
        public void CanCreate_FromScratch()
        {
            SchemaExport schemaCreator = new SchemaExport(CreateConfiguration(xml1));
            schemaCreator.Execute((s) =>
                {
                    Debug.WriteLine(" === Create === ");
                    Debug.WriteLine(s);
                }, true, false);

            schemaCreator.Execute((s) =>
            {
                Debug.WriteLine(" === Drop === ");
                Debug.WriteLine(s);
            }, true, true);
        }

        [Test, Ignore("Requires sql server")]
        public void CanUpdate_IntProperty_WithoutClearingData()
        {
            var cfg1 = CreateConfiguration(xml1);
            SchemaExport schemaCreator = new SchemaExport(cfg1);
            schemaCreator.Execute((s) =>
            {
                Debug.WriteLine(" === Create === ");
                Debug.WriteLine(s);
            }, true, false);

            Cat c = new Cat { Name = "Kitty", Age = 7 };
            using (var s = cfg1.BuildSessionFactory().OpenSession())
            {
                s.Save(c);
            }

            var cfg2 = CreateConfiguration(xml2);
            SchemaUpdate schemaUpdater = new SchemaUpdate(cfg2);
            schemaUpdater.Execute((s) =>
            {
                Debug.WriteLine(" === Update === ");
                Debug.WriteLine(s);
            }, true);

            using (var s = cfg2.BuildSessionFactory().OpenSession())
            {
                Cat c2 = s.Get<Cat>(c.ID);
                Assert.That(c2.Name, Is.EqualTo(c.Name));
                Assert.That(c2.Age, Is.EqualTo(0));
            }

            schemaCreator.Execute((s) =>
            {
                Debug.WriteLine(" === Drop === ");
                Debug.WriteLine(s);
            }, true, true);
        }

        [Test, Ignore("Requires sql server")]
        public void CanUpdate_FromScratch()
        {
            SchemaUpdate schemaUpdater = new SchemaUpdate(CreateConfiguration(xml2));
            schemaUpdater.Execute((s) =>
            {
                Debug.WriteLine(" === Update === ");
                Debug.WriteLine(s);
            }, true);

            SchemaExport schemaCreator = new SchemaExport(CreateConfiguration(xml2));
            schemaCreator.Execute((s) =>
            {
                Debug.WriteLine(" === Drop === ");
                Debug.WriteLine(s);
            }, true, true);
        }



        NHibernate.Cfg.Configuration CreateConfiguration(string xml)
        {
            NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration();
            cfg.Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.SqlClientDriver).AssemblyQualifiedName;
            cfg.Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.MsSql2008Dialect).AssemblyQualifiedName;
            cfg.Properties[NHibernate.Cfg.Environment.ProxyFactoryFactoryClass] = "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle";
            cfg.Properties[NHibernate.Cfg.Environment.ConnectionStringName] = "SqlConnection";
            cfg.Properties[NHibernate.Cfg.Environment.ConnectionProvider] = "NHibernate.Connection.DriverConnectionProvider";

            cfg.AddXmlString(xml);
            return cfg;
        }

        string xml1 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<hibernate-mapping xmlns=""urn:nhibernate-mapping-2.2"" default-lazy=""false"">
    <class name=""N2.Tests.SchemaExporting.Cat, N2.Tests"" table=""animals"">
        <id name=""ID"" column=""ID"" type=""Int32"" unsaved-value=""0"">
            <generator class=""native""/>
        </id>
        <property name=""Name"" type=""String"" not-null=""true"" length=""50"" />
    </class>
</hibernate-mapping>";

        string xml2 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<hibernate-mapping xmlns=""urn:nhibernate-mapping-2.2"" default-lazy=""false"">
    <class name=""N2.Tests.SchemaExporting.Cat, N2.Tests"" table=""animals"">
        <id name=""ID"" column=""ID"" type=""Int32"" unsaved-value=""0"">
            <generator class=""native""/>
        </id>
        <property name=""Name"" type=""String"" not-null=""true"" length=""50"" />
        <property name=""Age"" type=""Int32"" not-null=""true"" />
    </class>
</hibernate-mapping>";

    }
}
