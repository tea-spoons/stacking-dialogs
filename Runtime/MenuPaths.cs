namespace TeaSpoons.StackingDialogs
{
    internal static class MenuPaths
    {
        /// <summary>
        /// The shared root of the TeaSpoons menus. Taken from package-core when the project has it.
        /// </summary>
#if TEASPOONS_PACKAGE_CORE
        public const string Root = PackageCore.Menus.RootItem;
#else
        public const string Root = "TeaSpoons/";
#endif
    }
}
