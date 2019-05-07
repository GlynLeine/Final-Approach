using System.Collections.Generic;

namespace GLXEngine.Core
{
	public class Collider
	{
        public Vector2 m_minimumTranslationVec = new Vector2();

        public List<Collider> m_children = new List<Collider>();

		public Collider ()
		{
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														HitTest()
		//------------------------------------------------------------------------------------------------------------------------		
		public virtual bool HitTest (ref Collider other) {
			return false;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														HitTest()
		//------------------------------------------------------------------------------------------------------------------------		
		public virtual bool HitTestPoint (float x, float y) {
			return false;
		}
	}
}

