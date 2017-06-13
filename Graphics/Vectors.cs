#region MIT

// 
// GMare.
// Copyright (C) 2008 - 2010 Pyxosoft
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//

#endregion

using System;
using System.Drawing;

namespace GMare.Graphics
{
    public class Line
    {
        #region Fields

        private Point _start = Point.Empty;
        private Point _end = Point.Empty;
        private Color _color = Color.White;

        #endregion

        #region Properties

        public Point Start
        {
            get { return _start; }
        }

        public Point End
        {
            get { return _end; }
        }

        public Color Color
        {
            get { return _color; }
        }

        #endregion

        #region Constructor | Destructor

        public Line(Point start, Point end, Color color)
        {
            _start = start;
            _end = end;
            _color = color;
        }

        #endregion
    }

    public class Quad
    {
        #region Fields
        
        private uint _textureId = 0;                           // The texture id.
        private PointF[] _textureCoordinates = new PointF[4];  // The tetxure coordinates.
        private PointF[] _vertices = new PointF[4];            // The vertices.
        private Color _color = Color.White;                    // The blending color.

        #endregion

        #region Properties

        /// <summary>
        /// Get the texture id for the quad.
        /// </summary>
        public uint TextureId
        {
            get { return _textureId; }
        }

        /// <summary>
        /// Gets the quad's texture coordinates.
        /// </summary>
        public PointF[] TextureCoordinates
        {
            get { return _textureCoordinates; }
        }

        /// <summary>
        /// Gets the quad's vertices.
        /// </summary>
        public PointF[] Vertices
        {
            get { return _vertices; }
        }

        /// <summary>
        /// Gets the blend color of the quad.
        /// </summary>
        public Color Color
        {
            get { return _color; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new quad.
        /// </summary>
        /// <param name="textureId"></param>
        /// <param name="vertices"></param>
        /// <param name="textureCoordinates"></param>
        /// <param name="color"></param>
        public Quad(ResTexture texture, RectangleF destinationRectangle, RectangleF sourceRectangle, PointF scale, float angle, Color color)
        {
            _textureId = texture.TextureId;
            _color = color;
            
            // Set vertices.
            _vertices[0] = new PointF(destinationRectangle.X, destinationRectangle.Y);
            _vertices[1] = new PointF(destinationRectangle.Width + _vertices[0].X, destinationRectangle.Y);
            _vertices[2] = new PointF(destinationRectangle.Width + _vertices[0].X, destinationRectangle.Height + _vertices[0].Y);
            _vertices[3] = new PointF(destinationRectangle.X, destinationRectangle.Height + _vertices[0].Y);

            var texCoordX = sourceRectangle.X / texture.Width;
            var texCoordY = sourceRectangle.Y / texture.Height;

            var texCoordWidth = (sourceRectangle.Width / texture.Width) * scale.X;
            var texCoordHeight = (sourceRectangle.Height / texture.Height) * scale.Y;

            // Set texture coordinates.
            _textureCoordinates[0] = new PointF(texCoordX, texCoordY);
            _textureCoordinates[1] = new PointF(texCoordX + texCoordWidth, texCoordY);
            _textureCoordinates[2] = new PointF(texCoordX + texCoordWidth, texCoordY + texCoordHeight);
            _textureCoordinates[3] = new PointF(texCoordX, texCoordY + texCoordHeight);
        }

        #endregion
    }
}
