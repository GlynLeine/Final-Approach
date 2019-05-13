using System.Drawing;
using GLXEngine.Core;

namespace GLXEngine.Rendering.UI
{
    public class UIOverlay : EasyDraw
    {
        public UIOverlay(Bitmap bitmap) : base(bitmap) { }

        public UIOverlay(string filename) : base(filename) { }

        public UIOverlay(int width, int height) : base(width, height) { }

        protected override Collider createCollider() { return null; }
    }
}
