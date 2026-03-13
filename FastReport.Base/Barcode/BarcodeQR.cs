using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using FastReport.Barcode.QRCode;
using FastReport.Utils;
using System.Drawing.Drawing2D;

namespace FastReport.Barcode
{
    /// <summary>
    /// Specifies the QR code error correction level.
    /// </summary>
    public enum QRCodeErrorCorrection
    {
        /// <summary>
        /// L = ~7% correction.
        /// </summary>
        L,

        /// <summary>
        /// M = ~15% correction.
        /// </summary>
        M,

        /// <summary>
        /// Q = ~25% correction.
        /// </summary>
        Q,

        /// <summary>
        /// H = ~30% correction.
        /// </summary>
        H
    }

    /// <summary>
    /// Specifies the QR Code encoding.
    /// </summary>
    public enum QRCodeEncoding
    {
        /// <summary>
        /// UTF-8 encoding.
        /// </summary>
        UTF8,
        /// <summary>
        /// ISO 8859-1 encoding.
        /// </summary>
        ISO8859_1,
        /// <summary>
        /// Shift_JIS encoding.
        /// </summary>
        Shift_JIS,
        /// <summary>
        /// Windows-1251 encoding.
        /// </summary>
        Windows_1251,
        /// <summary>
        /// cp866 encoding.
        /// </summary>
        cp866
    }

    /// <summary>
    /// Specifies the visual shape of individual modules (dots) in the QR code.
    /// </summary>
    public enum QrModuleShape
    {
        /// <summary>
        /// Standard square modules (classic QR code appearance).
        /// </summary>
        Rectangle,

        /// <summary>
        /// Circular modules.
        /// </summary>
        Circle,

        /// <summary>
        /// Diamond-shaped (rhombus) modules.
        /// </summary>
        Diamond,

        /// <summary>
        /// Square modules with rounded corners.
        /// </summary>
        RoundedSquare,

        /// <summary>
        /// Horizontal capsule-shaped modules (rectangle with semicircular ends on left and right).
        /// </summary>
        PillHorizontal,

        /// <summary>
        /// Vertical capsule-shaped modules (rectangle with semicircular ends on top and bottom).
        /// </summary>
        PillVertical,

        /// <summary>
        /// Plus-shaped ("+") modules.
        /// </summary>
        Plus,

        /// <summary>
        /// Regular hexagonal modules.
        /// </summary>
        Hexagon,

        /// <summary>
        /// Star-shaped modules with multiple radial points.
        /// </summary>
        Star,

        /// <summary>
        /// Snowflake-like modules with radial symmetry.
        /// </summary>
        Snowflake
    }

    /// <summary>
    /// Creates the 2D QR code barcode.
    /// </summary>
    public class BarcodeQR : Barcode2DBase
    {
        #region Fields
        private QRCodeErrorCorrection errorCorrection;
        private QRCodeEncoding encoding;
        private bool quietZone;
        private ByteMatrix matrix;
        private const int PixelSize = 4;

        private QrModuleShape shape;
        private int rotation;
        private bool useThinModules;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the error correction.
        /// </summary>
        [DefaultValue(QRCodeErrorCorrection.L)]
        public QRCodeErrorCorrection ErrorCorrection
        {
            get { return errorCorrection; }
            set { errorCorrection = value; }
        }

        /// <summary>
        /// Gets or sets the encoding used for text conversion.
        /// </summary>
        [DefaultValue(QRCodeEncoding.UTF8)]
        public QRCodeEncoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating that quiet zone must be shown.
        /// </summary>
        [DefaultValue(true)]
        public bool QuietZone
        {
            get { return quietZone; }
            set { quietZone = value; }
        }

        /// <summary>
        /// Gets or sets the visual shape of QR code modules.
        /// </summary>
        [DefaultValue(QrModuleShape.Rectangle)]
        public QrModuleShape Shape
        {
            get { return shape; }
            set { shape = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether non-finder modules should be rendered with thin (inset) appearance.
        /// When set to <see langword="true"/>, modules are drawn smaller than their cell with padding;
        /// when <see langword="false"/>, modules fill the entire cell (classic look).
        /// </summary>
        [DefaultValue(false)]
        public bool UseThinModules
        {
            get { return useThinModules; }
            set { useThinModules = value; }
        }

        /// <summary>
        /// Gets or sets the rotation angle (in degrees) applied to rotational module shapes.
        /// Supported shapes - Star, Hexagon, Snowflake.
        /// This property has no effect on non-rotational shapes (e.g., Rectangle, Circle, Diamond, etc.).
        /// </summary>
        [DefaultValue(0)]
        public int Angle
        {
            get { return rotation; }
            set { rotation = value; }
        }
        #endregion

        #region Private Methods
        private ErrorCorrectionLevel GetErrorCorrectionLevel()
        {
            // for swissQR QRCodeErrorCorrection only M
            if (text.StartsWith("SPC"))
                errorCorrection = QRCodeErrorCorrection.M;

            switch (errorCorrection)
            {
                case QRCodeErrorCorrection.L:
                    return ErrorCorrectionLevel.L;

                case QRCodeErrorCorrection.M:
                    return ErrorCorrectionLevel.M;

                case QRCodeErrorCorrection.Q:
                    return ErrorCorrectionLevel.Q;

                case QRCodeErrorCorrection.H:
                    return ErrorCorrectionLevel.H;
            }

            return ErrorCorrectionLevel.L;
        }

        private string GetEncoding()
        {
            switch (encoding)
            {
                case QRCodeEncoding.UTF8:
                    return "UTF-8";

                case QRCodeEncoding.ISO8859_1:
                    return "ISO-8859-1";

                case QRCodeEncoding.Shift_JIS:
                    return "Shift_JIS";

                case QRCodeEncoding.Windows_1251:
                    return "Windows-1251";

                case QRCodeEncoding.cp866:
                    return "cp866";
            }

            return "";
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Assign(BarcodeBase source)
        {
            base.Assign(source);
            BarcodeQR src = source as BarcodeQR;

            ErrorCorrection = src.ErrorCorrection;
            Encoding = src.Encoding;
            QuietZone = src.QuietZone;
            Shape = src.Shape;
            UseThinModules = src.UseThinModules;
            Angle = src.Angle;
        }

        internal override void Serialize(FastReport.Utils.FRWriter writer, string prefix, BarcodeBase diff)
        {
            base.Serialize(writer, prefix, diff);
            BarcodeQR c = diff as BarcodeQR;

            if (c == null || ErrorCorrection != c.ErrorCorrection)
                writer.WriteValue(prefix + "ErrorCorrection", ErrorCorrection);
            if (c == null || Encoding != c.Encoding)
                writer.WriteValue(prefix + "Encoding", Encoding);
            if (c == null || QuietZone != c.QuietZone)
                writer.WriteBool(prefix + "QuietZone", QuietZone);
            if (c == null || showMarker != c.showMarker)
                writer.WriteBool(prefix + "ShowMarker", showMarker);
            if (c == null || Shape != c.Shape)
                writer.WriteValue(prefix + "Shape", Shape);
            if (c == null || UseThinModules != c.UseThinModules)
                writer.WriteBool(prefix + "UseThinModules", UseThinModules);
            if (c == null || Angle != c.Angle)
                writer.WriteValue(prefix + "Angle", Angle);
        }

        internal override void Initialize(string text, bool showText, int angle, float zoom, bool showMarker)
        {
            base.Initialize(text, showText, angle, zoom, showMarker);
            matrix = QRCodeWriter.encode(base.text, 0, 0, GetErrorCorrectionLevel(), GetEncoding(), QuietZone);
        }

        internal override SizeF CalcBounds()
        {
            int textAdd = showText ? (int)(FontHeight) : 0;
            return new SizeF(matrix.Width * PixelSize, matrix.Height * PixelSize + textAdd);
        }

        internal override void Draw2DBarcode(IGraphics g, float kx, float ky)
        {
            Brush brush = new SolidBrush(Color);
            float scale = 1.25f;

            const float paddingRatio = 0.1f; // 10% padding for UseThinModules
            float paddingX = PixelSize * paddingRatio * kx;
            float paddingY = PixelSize * paddingRatio * ky;

            if (text.StartsWith("SPC"))
            {
                Shape = QrModuleShape.Rectangle;
                UseThinModules = false;
            }

            for (int y = 0; y < matrix.Height; y++)
            {
                for (int x = 0; x < matrix.Width; x++)
                {
                    if (matrix.get_Renamed(x, y) == 0)
                    {
                        float moduleX = x * PixelSize * kx;
                        float moduleY = y * PixelSize * ky;
                        float moduleWidth = PixelSize * kx;
                        float moduleHeight = PixelSize * ky;

                        bool isInFinder = IsInFinderPattern(x, y);

                        float renderX, renderY, renderWidth, renderHeight;

                        if (isInFinder)
                        {
                            // finder Pattern always thick (without indentation)
                            renderX = moduleX;
                            renderY = moduleY;
                            renderWidth = moduleWidth;
                            renderHeight = moduleHeight;
                        }
                        else
                        {
                            // other modules - depending on UseThinModules
                            if (UseThinModules)
                            {
                                renderX = moduleX + paddingX;
                                renderY = moduleY + paddingY;
                                renderWidth = moduleWidth - 2 * paddingX;
                                renderHeight = moduleHeight - 2 * paddingY;
                            }
                            else
                            {
                                renderX = moduleX;
                                renderY = moduleY;
                                renderWidth = moduleWidth;
                                renderHeight = moduleHeight;
                            }
                        }

                        switch (Shape)
                        {
                            case QrModuleShape.Rectangle:
                                g.FillRectangle(brush, renderX, renderY, renderWidth, renderHeight);
                                break;

                            case QrModuleShape.Circle:
                                scale = 1.05f;

                                float scaledDx = renderWidth * scale;
                                float scaledDy = renderHeight * scale;

                                // offset to stay centered
                                float offsetX = (scaledDx - renderWidth) / 2;
                                float offsetY = (scaledDy - renderHeight) / 2;

                                float newX = renderX - offsetX;
                                float newY = renderY - offsetY;

                                g.FillEllipse(brush, newX, newY, scaledDx, scaledDy);
                                break;

                            case QrModuleShape.Diamond:
                                var diamondPoints = CreateDiamondPoints(renderX, renderY, renderWidth, renderHeight, scale);
                                g.FillPolygon(brush, diamondPoints);
                                break;

                            case QrModuleShape.RoundedSquare:

                                // which corners should be rounded
                                bool topLeftRound, topRightRound, bottomRightRound, bottomLeftRound;

                                if (UseThinModules && !isInFinder)
                                {
                                    // all corners are rounded
                                    topLeftRound = topRightRound = bottomRightRound = bottomLeftRound = true;
                                }
                                else
                                {
                                    // adaptive rounding

                                    // coordinate offsets relative to the current module (x, y):
                                    //x = -1 - one step to the left
                                    //x = 0 - remain in the same column
                                    //x = +1 - one step to the right
                                    //y = -1 - one step up
                                    //y = 0 - remain in the same line
                                    //y = +1 - one step down

                                    topLeftRound = (!IsNeighborDark(x, y, -1, 0) || !IsNeighborDark(x, y, 0, -1)) || !IsNeighborDark(x, y, -1, -1);
                                    topRightRound = (!IsNeighborDark(x, y, 1, 0) || !IsNeighborDark(x, y, 0, -1)) || !IsNeighborDark(x, y, 1, -1);
                                    bottomRightRound = (!IsNeighborDark(x, y, 1, 0) || !IsNeighborDark(x, y, 0, 1)) || !IsNeighborDark(x, y, 1, 1);
                                    bottomLeftRound = (!IsNeighborDark(x, y, -1, 0) || !IsNeighborDark(x, y, 0, 1)) || !IsNeighborDark(x, y, -1, 1);
                                }

                                using (var path = CreateRoundedRectanglePath(
                                renderX, renderY, renderWidth, renderHeight,
                                topLeftRound, topRightRound, bottomRightRound, bottomLeftRound))
                                {
                                    g.FillPath(brush, path);
                                }
                                break;

                            case QrModuleShape.PillHorizontal:

                                // check for dark neighbors on the left and right
                                bool hasLeftNeighbor = IsNeighborDark(x, y, -1, 0);
                                bool hasRightNeighbor = IsNeighborDark(x, y, 1, 0);

                                using (var path = CreateHorizontalPillPath(renderX, renderY, renderWidth, renderHeight, hasLeftNeighbor, hasRightNeighbor))
                                {
                                    g.FillPath(brush, path);
                                }
                                break;

                            case QrModuleShape.PillVertical:
                                // check for dark neighbors on the top and bottom
                                bool hasTopNeighbor = IsNeighborDark(x, y, 0, -1);
                                bool hasBottomNeighbor = IsNeighborDark(x, y, 0, 1);

                                using (var path = CreateVerticalPillPath(renderX, renderY, renderWidth, renderHeight, hasTopNeighbor, hasBottomNeighbor))
                                {
                                    g.FillPath(brush, path);
                                }
                                break;

                            case QrModuleShape.Plus:
                                float cx = renderX + renderWidth / 2;
                                float cy = renderY + renderHeight / 2;

                                // horizontal stripe: X-width, Y-thickness
                                float horizWidth = renderWidth / 2 * scale; // half the width of the module
                                float horizHeight = renderHeight * 0.2f; // 20% of the module height

                                // vertical stripe: Y-width, X-thickness
                                float vertWidth = renderWidth * 0.2f; // 20% of the module width
                                float vertHeight = renderHeight / 2 * scale; // half the height of the module

                                // horizontal stripe
                                g.FillRectangle(brush,
                                    cx - horizWidth, cy - horizHeight,
                                    2 * horizWidth, 2 * horizHeight);

                                // vertical stripe
                                g.FillRectangle(brush,
                                    cx - vertWidth, cy - vertHeight,
                                    2 * vertWidth, 2 * vertHeight);

                                break;

                            case QrModuleShape.Hexagon:
                                float rx = renderWidth / 2 * scale;
                                float ry = renderHeight / 2 * scale;

                                var hexPoints = CreateHexagonPoints(
                                    renderX + renderWidth / 2,
                                    renderY + renderHeight / 2,
                                    rx,
                                    ry);
                                g.FillPolygon(brush, hexPoints);
                                break;

                            case QrModuleShape.Star:
                                float outerRx = renderWidth / 2 * scale;
                                float outerRy = renderHeight / 2 * scale;

                                var starPoints = CreateStarPoints(
                                    renderX + renderWidth / 2,
                                    renderY + renderHeight / 2,
                                    outerRx,
                                    outerRy
                                );
                                g.FillPolygon(brush, starPoints);
                                break;

                            case QrModuleShape.Snowflake:
                                var snowflakePoints = CreateSnowflakePoints(
                                    renderX + renderWidth / 2,
                                    renderY + renderHeight / 2,
                                    renderWidth / 2,
                                    renderHeight / 2
                                );

                                g.FillPolygon(brush, snowflakePoints);
                                break;
                        }
                    }
                }
            }
            brush.Dispose();
        }

        /// <summary>
        /// Creates the vertices of a diamond (rotated square) centered within the specified rectangle.
        /// </summary>
        /// <param name="x">X-coordinate of the top-left corner of the bounding rectangle.</param>
        /// <param name="y">Y-coordinate of the top-left corner of the bounding rectangle.</param>
        /// <param name="width">Width of the bounding rectangle.</param>
        /// <param name="height">Height of the bounding rectangle.</param>
        /// <param name="scale">Scaling factor applied uniformly from the center.</param>
        /// <returns>An array of 5 points defining the diamond's vertices in clockwise order, with the first point repeated at the end for polygon closure.</returns>
        private static PointF[] CreateDiamondPoints(float x, float y, float width, float height, float scale)
        {
            // center of the module
            float cx = x + width / 2;
            float cy = y + height / 2;
            var points = new PointF[]
            {
                new(cx, y), // top
                new(x + width, cy), // right
                new(cx, y + height), // bottom
                new(x, cy), // left
                new(cx, y) // closing
            };

            // increase of each point about the center
            for (int i = 0; i < points.Length; i++)
            {
                float dx = points[i].X - cx;
                float dy = points[i].Y - cy;
                points[i] = new PointF(cx + dx * scale, cy + dy * scale);
            }
            return points;
        }

        /// <summary>
        /// Creates a <see cref="GraphicsPath"/> for a rounded rectangle with explicit control over which corners are rounded.
        /// </summary>
        /// <param name="x">X-coordinate of the top-left corner of the rectangle.</param>
        /// <param name="y">Y-coordinate of the top-left corner of the rectangle.</param>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        /// <param name="topLeftRound">If <see langword="true"/>, the top-left corner is rounded.</param>
        /// <param name="topRightRound">If <see langword="true"/>, the top-right corner is rounded.</param>
        /// <param name="bottomRightRound">If <see langword="true"/>, the bottom-right corner is rounded.</param>
        /// <param name="bottomLeftRound">If <see langword="true"/>, the bottom-left corner is rounded.</param>
        /// <returns>A <see cref="GraphicsPath"/> representing the rounded rectangle.</returns>
        private static GraphicsPath CreateRoundedRectanglePath(float x, float y, float width, float height,
        bool topLeftRound, bool topRightRound, bool bottomRightRound, bool bottomLeftRound)
        {
            // prevent overlap in non-square (stretched) modules
            float radius = Math.Min(width, height) / 4;
            if (radius < 1) radius = 1;

            var path = new GraphicsPath();

            // start at top-left point
            float currentX = topLeftRound ? x + radius : x;
            float currentY = y;
            path.StartFigure(); // explicitly start the path

            // top edge to top-right corner
            float nextX = topRightRound ? x + width - radius : x + width;
            path.AddLine(currentX, currentY, nextX, currentY);
            currentX = nextX;

            // top-right arc
            if (topRightRound)
            {
                path.AddArc(x + width - radius * 2, y, radius * 2, radius * 2, 270, 90);
                currentX = x + width;
                currentY = y + radius;
            }

            // right edge to bottom-right corner
            nextX = x + width;
            float nextY = bottomRightRound ? y + height - radius : y + height;
            path.AddLine(currentX, currentY, nextX, nextY);
            currentY = nextY;

            // bottom-right arc
            if (bottomRightRound)
            {
                path.AddArc(x + width - radius * 2, y + height - radius * 2, radius * 2, radius * 2, 0, 90);
                currentX = x + width - radius;
                currentY = y + height;
            }

            // bottom edge to bottom-left corner
            nextX = bottomLeftRound ? x + radius : x;
            path.AddLine(currentX, currentY, nextX, currentY);
            currentX = nextX;

            // bottom-left arc
            if (bottomLeftRound)
            {
                path.AddArc(x, y + height - radius * 2, radius * 2, radius * 2, 90, 90);
                currentX = x;
                currentY = y + height - radius;
            }

            // left edge back to top-left
            nextY = topLeftRound ? y + radius : y;
            path.AddLine(currentX, currentY, x, nextY);

            // top-left arc
            if (topLeftRound)
            {
                path.AddArc(x, y, radius * 2, radius * 2, 180, 90);
            }

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Creates a <see cref="GraphicsPath"/> for a horizontal pill shape, adapting its ends based on the presence of adjacent dark modules.
        /// The pill consists of a rectangle with semicircular caps on the left and/or right.
        /// If a dark neighbor exists on a side, that end becomes flat (rectangular); otherwise, it remains rounded.
        /// </summary>
        /// <param name="x">X-coordinate of the top-left corner of the module</param>
        /// <param name="y">Y-coordinate of the top-left corner of the module</param>
        /// <param name="width">Width of the module</param>
        /// <param name="height">Height of the module</param>
        /// <param name="hasLeftNeighbor"><see langword="true"/> if there is a dark module to the left.</param>
        /// <param name="hasRightNeighbor"><see langword="true"/> if there is a dark module to the right.</param>
        /// <returns>A <see cref="GraphicsPath"/> representing the pill shape</returns>
        private static GraphicsPath CreateHorizontalPillPath(float x, float y, float width, float height, bool hasLeftNeighbor, bool hasRightNeighbor)
        {
            // ensures a circular (not elliptical) shape even in stretched (non-square) modules
            float radius = width / 2;
            var path = new GraphicsPath();

            if (hasLeftNeighbor && hasRightNeighbor)
            {
                // neighbors on both sides — draw a rectangle
                path.AddRectangle(new RectangleF(x, y, width, height));
            }
            else if (hasLeftNeighbor && !hasRightNeighbor)
            {
                // left side straight, right side rounded
                path.AddLine(x, y, x, y + height);
                path.AddArc(x + width - radius * 2, y, radius * 2, height, -270, -180);
                path.CloseFigure();
            }
            else if (!hasLeftNeighbor && hasRightNeighbor)
            {
                // left side rounded, right side straight
                path.AddArc(x, y, radius * 2, height, 90, 180);
                path.AddLine(x + width, y, x + width, y + height);
                path.CloseFigure();
            }
            else
            {
                // no neighbors — rounded ends on both sides
                path.AddArc(x, y, radius * 2, height, 180, 180); // left semicircle
                path.AddArc(x + width - radius * 2, y, radius * 2, height, 0, 180); // right semicircle
                path.CloseFigure();
            }

            return path;
        }

        /// <summary>
        /// Creates a <see cref="GraphicsPath"/> for a vertical pill shape, adapting its top and bottom ends 
        /// based on the presence of adjacent dark modules.
        /// The pill consists of a rectangle with semicircular caps on the top and/or bottom.
        /// If a dark neighbor exists above or below, that end becomes flat (rectangular); otherwise, it remains rounded.
        /// </summary>
        /// <param name="x">X-coordinate of the top-left corner of the module</param>
        /// <param name="y">Y-coordinate of the top-left corner of the module</param>
        /// <param name="width">Width of the module</param>
        /// <param name="height">Height of the module</param>
        /// <param name="hasTopNeighbor"><see langword="true"/> if there is a dark module directly above</param>
        /// <param name="hasBottomNeighbor"><see langword="true"/> if there is a dark module directly below</param>
        /// <returns>A <see cref="GraphicsPath"/> representing the pill shape</returns>
        private static GraphicsPath CreateVerticalPillPath(float x, float y, float width, float height, bool hasTopNeighbor, bool hasBottomNeighbor)
        {
            // ensures a circular (not elliptical) shape even in stretched (non-square) modules
            float radius = height / 2;

            var path = new GraphicsPath();

            if (hasTopNeighbor && hasBottomNeighbor)
            {
                // neighbors on both sides — draw a rectangle
                path.AddRectangle(new RectangleF(x, y, width, height));
            }
            else if (hasTopNeighbor && !hasBottomNeighbor)
            {
                // top side straight, bottom side rounded
                path.AddLine(x, y, x + width, y);
                path.AddArc(x, y + height - radius * 2, width, radius * 2, 0, 180);
                path.CloseFigure();
            }
            else if (!hasTopNeighbor && hasBottomNeighbor)
            {
                // top side rounded, bottom side straight
                path.AddArc(x, y, width, radius * 2, 0, -180);
                path.AddLine(x, y + height, x + width, y + height);
                path.CloseFigure();
            }
            else
            {
                // no neighbors — rounded ends on both sides
                path.AddArc(x, y, width, radius * 2, 270, 180); // top semicircle
                path.AddArc(x, y + height - radius * 2, width, radius * 2, 90, 180); // bottom semicircle
                path.CloseFigure();
            }
            return path;
        }

        /// <summary>
        /// Creates the vertices of a hexagon centered at the specified coordinates.
        /// The hexagon has a flat side on top by default and can be rotated using the <see cref="Angle"/> property.
        /// </summary>
        /// <param name="centerX">X-coordinate of the hexagon's center.</param>
        /// <param name="centerY">Y-coordinate of the hexagon's center.</param>
        /// <param name="radiusX">Horizontal radius (distance from center to a vertex along the X-axis).</param>
        /// <param name="radiusY">Vertical radius (distance from center to a vertex along the Y-axis).</param>
        /// <returns>An array of 6 points representing the hexagon's vertices in clockwise order.</returns>
        private PointF[] CreateHexagonPoints(float centerX, float centerY, float radiusX, float radiusY)
        {
            double rotAngle = Angle * Math.PI / 180.0;
            var points = new PointF[6];
            for (int i = 0; i < 6; i++)
            {
                double pointAngle = Math.PI / 3 * i - Math.PI / 6 + rotAngle; // flat side on top
                points[i] = new PointF(
                    centerX + (float)(radiusX * Math.Cos(pointAngle)),
                    centerY + (float)(radiusY * Math.Sin(pointAngle))
                );
            }
            return points;
        }

        /// <summary>
        /// Creates the vertices of a 5-pointed star centered at the specified coordinates.
        /// The star is oriented with one point facing upward by default and can be rotated 
        /// using the <see cref="Angle"/> property.
        /// </summary>
        /// <param name="centerX">X-coordinate of the star's center.</param>
        /// <param name="centerY">Y-coordinate of the star's center.</param>
        /// <param name="outerRadiusX">Horizontal radius of the outer (tip) vertices.</param>
        /// <param name="outerRadiusY">Vertical radius of the outer (tip) vertices.</param>
        /// <returns>An array of 10 points (5 outer + 5 inner) representing the star's vertices in alternating order.</returns>
        /// <remarks>
        /// Inner vertices are scaled to 60% of the outer radius to form the star's indentations.
        /// </remarks>
        private PointF[] CreateStarPoints(float centerX, float centerY, float outerRadiusX, float outerRadiusY)
        {
            double rotAngle = Angle * Math.PI / 180.0;
            var points = new PointF[10]; // 5 external + 5 internal points

            for (int i = 0; i < 10; i++)
            {
                double pointAngle = Math.PI / 5 * i - Math.PI / 2 + rotAngle; // start from the top

                float ratio = i % 2 == 0 ? 1.0f : 0.6f;

                float rx = outerRadiusX * ratio;
                float ry = outerRadiusY * ratio;

                points[i] = new PointF(
                    centerX + (float)(rx * Math.Cos(pointAngle)),
                    centerY + (float)(ry * Math.Sin(pointAngle))
                );
            }
            return points;
        }

        /// <summary>
        /// Creates a snowflake-shaped polygon as an array of points.
        /// The snowflake has 6 primary lobes and is designed to fit within a module cell,
        /// with optional scaling and rotation.
        /// </summary>
        /// <param name="centerX">X-coordinate of the snowflake center.</param>
        /// <param name="centerY">Y-coordinate of the snowflake center.</param>
        /// <param name="sx">Horizontal scale factor (half-width of the bounding box).</param>
        /// <param name="sy">Vertical scale factor (half-height of the bounding box).</param>
        /// <returns>An array of points defining the snowflake contour, ready for FillPolygon.</returns>
        private PointF[] CreateSnowflakePoints(float centerX, float centerY, float sx, float sy)
        {  
            int pointCount = 120;
            var points = new PointF[pointCount];

            bool useRotation = Angle != 0;
            float cosA = 1f, sinA = 0f;
            if (useRotation)
            {
                double angleRad = Angle * Math.PI / 180.0;
                cosA = (float)Math.Cos(angleRad);
                sinA = (float)Math.Sin(angleRad);
            }

            double angleStep = Math.PI * 2.0 / pointCount;

            for (int i = 0; i < pointCount; i++)
            {
                double pointAngle = i * angleStep;

                float distance = 1.0f + 0.4f * (float)Math.Cos(6 * pointAngle + Math.PI); // 6 lobes

                float dx = distance * (float)Math.Cos(pointAngle);
                float dy = distance * (float)Math.Sin(pointAngle);

                dx *= sx;
                dy *= sy;

                // apply rotation if necessary
                if (useRotation)
                {
                    float newX = dx * cosA - dy * sinA;
                    float newY = dx * sinA + dy * cosA;
                    dx = newX;
                    dy = newY;
                }

                points[i] = new PointF(centerX + dx, centerY + dy);
            }

            return points;
        }

        /// <summary>
        /// Determines whether the given matrix coordinates (x, y) belong to one of the three standard QR code Finder Patterns.
        /// </summary>
        /// <param name="x">X-coordinate in the QR code matrix.</param>
        /// <param name="y">Y-coordinate in the QR code matrix.</param>
        /// <returns>
        /// <see langword="true"/> if (x, y) lies within any of the three 7×7 Finder Patterns
        /// (top-left, top-right, or bottom-left); otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Finder Patterns are fixed 7×7 modules located at three corners of the QR code.
        /// If a quiet zone is enabled (<see cref="QuietZone"/> = <see langword="true"/>), it is assumed to be 4 modules wide,
        /// and the pattern coordinates are offset accordingly.
        /// </para>
        /// </remarks>
        private bool IsInFinderPattern(int x, int y)
        {
            int quiet = QuietZone ? 4 : 0;
            int width = matrix.Width;
            int height = matrix.Height;

            // Top-left Finder Pattern: rows [quiet, quiet+6], columns [quiet, quiet+6]
            if (x >= quiet && x <= quiet + 6 && y >= quiet && y <= quiet + 6)
                return true;

            // Top-right Finder Pattern: columns [width - quiet - 7, width - quiet - 1], rows [quiet, quiet + 6]
            if (x >= width - quiet - 7 && x <= width - quiet - 1 && y >= quiet && y <= quiet + 6)
                return true;

            // Bottom-left Finder Pattern: columns [quiet, quiet + 6], rows [height - quiet - 7, height - quiet - 1]
            if (x >= quiet && x <= quiet + 6 && y >= height - quiet - 7 && y <= height - quiet - 1)
                return true;

            return false;
        }

        /// <summary>
        /// Checks whether the neighboring module in the specified direction is dark (black).
        /// </summary>
        /// <param name="x">X-coordinate of the current module.</param>
        /// <param name="y">Y-coordinate of the current module.</param>
        /// <param name="dx">Offset in the X direction (-1 = left, 0 = same, +1 = right).</param>
        /// <param name="dy">Offset in the Y direction (-1 = up, 0 = same, +1 = down).</param>
        /// <returns>
        /// <see langword="true"/> if the neighbor at (x + dx, y + dy) exists and is dark;
        /// <see langword="false"/> if the neighbor is outside the matrix bounds or is light (white).
        /// </returns>
        /// <remarks>
        /// In this implementation, a module value of 0 represents a dark (black) module,
        /// while 1 (or any other value) represents a light (white) module.
        /// Positions outside the matrix are treated as light (non-dark).
        /// </remarks>
        private bool IsNeighborDark(int x, int y, int dx, int dy)
        {
            int nx = x + dx;
            int ny = y + dy;
            if (nx < 0 || nx >= matrix.Width || ny < 0 || ny >= matrix.Height)
                return false;
            return matrix.get_Renamed(nx, ny) == 0;
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeQR"/> class with default settings.
        /// </summary>
        public BarcodeQR()
        {
            Encoding = QRCodeEncoding.UTF8;
            QuietZone = true;
        }
    }
}
