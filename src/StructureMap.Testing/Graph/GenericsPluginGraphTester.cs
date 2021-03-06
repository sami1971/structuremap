using System;
using System.Linq;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Bugs;
using StructureMap.Testing.GenericWidgets;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class GenericsPluginGraphTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        private void assertCanBeCast(Type pluginType, Type TPluggedType)
        {
            Assert.IsTrue(GenericsPluginGraph.CanBeCast(pluginType, TPluggedType));
        }

        private void assertCanNotBeCast(Type pluginType, Type TPluggedType)
        {
            Assert.IsFalse(GenericsPluginGraph.CanBeCast(pluginType, TPluggedType));
        }

        [Test]
        public void BuildAnInstanceManagerFromTemplatedPluginFamily()
        {
            var container = new Container(x => {
                x.For(typeof (IGenericService<>)).Use(typeof (GenericService<>)).Named("Default");
                x.For(typeof (IGenericService<>)).Add(typeof (SecondGenericService<>)).Named("Second");
                x.For(typeof (IGenericService<>)).Add(typeof (ThirdGenericService<>)).Named("Third");
            });


            var intService = container.GetInstance<IGenericService<int>>().ShouldBeOfType<GenericService<int>>();
            intService.GetT().ShouldEqual(typeof (int));

            container.GetInstance<IGenericService<int>>("Second").ShouldBeOfType<SecondGenericService<int>>();

            var stringService =
                (GenericService<string>) container.GetInstance<IGenericService<string>>();
            stringService.GetT().ShouldEqual(typeof (string));
        }

        [Test]
        public void BuildTemplatedFamilyWithOnlyOneTemplateParameter()
        {
            var pluginGraph = new PluginGraph();
            PluginFamily family = pluginGraph.Families[typeof (IGenericService<>)];
            family.AddType(typeof (GenericService<>), "Default");
            family.AddType(typeof (SecondGenericService<>), "Second");
            family.AddType(typeof (ThirdGenericService<>), "Third");

            PluginFamily templatedFamily1 = family.CreateTemplatedClone(new[] {typeof (int)});
            PluginFamily templatedFamily = templatedFamily1;

            Assert.IsNotNull(templatedFamily);
            Assert.AreEqual(typeof (IGenericService<int>), templatedFamily.PluginType);
        }

        [Test]
        public void Check_the_generic_plugin_family_expression()
        {
            var container =
                new Container(
                    r =>
                    {
                        r.For(typeof (IGenericService<>)).Use(
                            typeof (GenericService<>));
                    });

            container.GetInstance<IGenericService<string>>().ShouldBeOfType(typeof (GenericService<string>));
        }

        [Test]
        public void checking_can_be_cast()
        {
            assertCanBeCast(typeof (IOpenType<>), typeof (OpenType<>));
        }


        [Test]
        public void DirectImplementationOfInterfaceCanBeCast()
        {
            assertCanBeCast(typeof (IGenericService<>), typeof (GenericService<>));
            assertCanNotBeCast(typeof (IGenericService<>), typeof (SpecificService<>));
        }

        [Test]
        public void DirectInheritanceOfAbstractClassCanBeCast()
        {
            assertCanBeCast(typeof (BaseSpecificService<>), typeof (SpecificService<>));
        }

        [Test]
        public void ImplementationOfInterfaceFromBaseType()
        {
            assertCanBeCast(typeof (ISomething<>), typeof (SpecificService<>));
        }

        [Test]
        public void RecursiveImplementation()
        {
            assertCanBeCast(typeof (ISomething<>), typeof (SpecificService<>));
            assertCanBeCast(typeof (ISomething<>), typeof (GrandChildSpecificService<>));
        }

        [Test]
        public void RecursiveInheritance()
        {
            assertCanBeCast(typeof (BaseSpecificService<>), typeof (ChildSpecificService<>));
            assertCanBeCast(typeof (BaseSpecificService<>), typeof (GrandChildSpecificService<>));
        }
    }

    public interface IGenericService<T>
    {
    }

    public class GenericService<T> : IGenericService<T>
    {
        public Type GetT()
        {
            return typeof (T);
        }
    }

    public class SecondGenericService<T> : IGenericService<T>
    {
    }

    public class ThirdGenericService<T> : IGenericService<T>
    {
    }

    public interface ISomething<T>
    {
    }

    public interface ISomething2<T>
    {
    }

    public interface ISomething3<T>
    {
    }

    public abstract class BaseSpecificService<T> : ISomething<T>
    {
    }

    public class SpecificService<T> : BaseSpecificService<T>
    {
    }

    public class ChildSpecificService<T> : SpecificService<T>
    {
    }

    public class GrandChildSpecificService<T> : ChildSpecificService<T>
    {
    }


    public interface IGenericService3<T, U, V>
    {
    }

    public class GenericService3<T, U, V> : IGenericService3<T, U, V>
    {
        public Type GetT()
        {
            return typeof (T);
        }
    }

    public class SecondGenericService3<T, U, V> : IGenericService3<T, U, V>
    {
    }

    public class ThirdGenericService3<T, U, V> : IGenericService3<T, U, V>
    {
    }
}