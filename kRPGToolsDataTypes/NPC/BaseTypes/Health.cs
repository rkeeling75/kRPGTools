using System;
using kRPGToolsDataTypes.Interfaces;

namespace kRPGToolsDataTypes.NPC.BaseTypes
{
    public class Health :IHealthStatus
    {
         private static readonly Lazy<Health> _alive = new Lazy<Health>(() => new Health());
         public static Health Alive
        {
            get { return _alive.Value; }
        }
        private static readonly Lazy<Health> _dead = new Lazy<Health>(() => new Health());
        public static Health Dead
        {
            get { return _dead.Value; }
        }
        private static readonly Lazy<Health> _fertile = new Lazy<Health>(() => new Health());
        public static Health Fertile
        {
            get { return _fertile.Value; }
        }
        private static readonly Lazy<Health> _barren = new Lazy<Health>(() => new Health());
        public static Health Barren
        {
            get { return _barren.Value; }
        }
    }
}