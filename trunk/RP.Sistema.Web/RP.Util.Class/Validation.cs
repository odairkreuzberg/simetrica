using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace RP.Util.Class
{
    /// <summary>
    /// Certifica que uma lista tem o mínimo de elementos estipulados
    /// </summary>
    public class EnsureMinimumElementsAttribute : ValidationAttribute
    {
        private readonly int _count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MinimumCount">Quantidade mínima de elementos</param>
        public EnsureMinimumElementsAttribute(int MinimumCount)
        {
            _count = MinimumCount;
        }

        public override bool IsValid(object value)
        {
            var list = value as System.Collections.ICollection;
            if (list != null)
            {
                return list.Count >= _count;
            }
            return false;
        }
    }

    /// <summary>
    /// Certifica que uma lista tem exatamente a quantidade de elementos estipulada
    /// </summary>
    public class EnsureCountElementsAttribute : ValidationAttribute
    {
        private readonly int _count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Count">Quantidade exata de elementos</param>
        public EnsureCountElementsAttribute(int Count)
        {
            _count = Count;
        }

        public override bool IsValid(object value)
        {
            var list = value as System.Collections.ICollection;
            if (list != null)
            {
                return list.Count == _count;
            }
            return false;
        }
    }
}
