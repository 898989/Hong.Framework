
#if NET45
namespace Hong.Common.Extendsion
{
    //Method	                    ||Programming construct
    //TryInvokeMember	            ||Method
    //TryGetMember, TrySetMember	||Property or field
    //TryGetIndex, TrySetIndex	    ||Indexer
    //TryUnaryOperation	            ||Unary operator such as !
    //TryBinaryOperation	        ||Binary operator such as ==
    //TryConvert	                ||Conversion (cast) to another type
    //TryInvoke	                    ||Invocation on the object itself e.g., d("foo")
    public class Dynamic : DynamicObject
    {
        IDictionary<string, object> propertyCache = new Dictionary<string, object>();

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            bool canInvoke = base.TryInvokeMember(binder, args, out result);
            
            if (!canInvoke) 
                this.MethodMissing(binder.Name,out result,args);

            return true;
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (propertyCache.ContainsKey(binder.Name))
            {
                result = propertyCache[binder.Name];
                return true;
            }
            else
            {
                bool canGetMember = base.TryGetMember(binder, out result);

                if (!canGetMember)
                    throw new MissingMemberException(this.GetType().ToString(), new Exception(binder.Name));
                else
                    return canGetMember;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!propertyCache.ContainsKey(binder.Name))
                propertyCache.Add(binder.Name, value);
            else
                propertyCache[binder.Name] = value;
            
            return true;
        }

        public virtual void MethodMissing(string methodName, out object result, object[] parameters)
        {
            var className = GetType().ToString();
            throw new MissingMethodException(className, new Exception(methodName));
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return propertyCache.Keys;
        }

        public IEnumerable<string> GetStaticMemberNames()
        {
            return this.GetType().GetProperties().Select(pi => pi.Name);
        }

        public IEnumerable<string> GetAllMemberNames()
        {
            return GetDynamicMemberNames().Union(GetStaticMemberNames());
        }

        public bool RespondTo(string memberName) 
        {
            if (HasStaticMember(memberName))
                return true;

            return propertyCache.ContainsKey(memberName);
        }

        private bool HasStaticMember(string memberName) 
        {
            var property = this.GetType().GetProperty(memberName);

            return property == null ? false : true;
           
        }
    }
}
#endif
