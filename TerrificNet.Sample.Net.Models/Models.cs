

 
 

namespace TerrificNet.Sample.Net.Models
{
    namespace Components
    {
        public class Link
        {
            public string Url
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }
        }

        public class BreadcrumbModel
        {
            public System.Collections.Generic.IList<Link> Links
            {
                get;
                set;
            }
        }
    }

    namespace Components
    {
    }

    namespace Components
    {
    }

    namespace Components
    {
        public class LogoModel
        {
            public string LogoTitle
            {
                get;
                set;
            }
        }
    }

    namespace Components
    {
    }

    namespace Components
    {
        public class MetaheadModel
        {
            public string PageTitle
            {
                get;
                set;
            }
        }
    }

    namespace Components
    {
        public class SubplaceholderModel
        {
            public string PlaceholderKey="Sub"
            {
                get;
                set;
            }
        }
    }

    namespace Components
    {
    }

    namespace Views
    {
        public class NestedplaceholderModel
        {
            public string Text
            {
                get;
                set;
            }

            public string PlaceholderKey="Nested"
            {
                get;
                set;
            }

            public string PlaceholderKey="Startempty"
            {
                get;
                set;
            }
        }
    }

    namespace Views
    {
        public class LayoutModel
        {
            public string PartialTemplate="Components/Modules/MetaHead/Metahead"
            {
                get;
                set;
            }

            public string ModuleTemplate="Components/Modules/Logo"
            {
                get;
                set;
            }

            public string PlaceholderKey="Main"
            {
                get;
                set;
            }

            public string PartialTemplate="Components/Modules/MetaFoot/Metafoot"
            {
                get;
                set;
            }
        }
    }
}