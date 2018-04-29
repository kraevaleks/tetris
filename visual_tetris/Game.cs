using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace visual_tetris
{
    public enum FigureType
    {
        I = 0, O, T, S, Z, J, L
    }

    public class Figure
    {
        public Figure(FigureType type)
        {
            m_type = type;
            m_points = new Point[4]; 
            switch (type)
            {
                case FigureType.I:
                    for (int i = 0; i < 4; ++i)
                        m_points[i] = new Point(i, 1);
                    break;
                case FigureType.O:
                    for (int i = 0; i < 2; ++i)
                        for (int j = 0; j < 2; ++j)
                            m_points[i * 2 + j] = new Point(j + 1, i + 1);
                    break;
                case FigureType.T:
                    for (int i = 0; i < 3; ++i)
                        m_points[i] = new Point(i, 1);
                    m_points[3] = new Point(1, 2);
                    break;
                case FigureType.S:
                    for (int i = 0; i < 2; ++i)
                        m_points[i] = new Point(i + 1, 1);
                    for (int i = 0; i < 2; ++i)
                        m_points[i + 2] = new Point(i, 2);
                    break;
                case FigureType.Z:
                    for (int i = 0; i < 2; ++i)
                        m_points[i] = new Point(i, 1);
                    for (int i = 0; i < 2; ++i)
                        m_points[i + 2] = new Point(i + 1, 2);
                    break;
                case FigureType.J:
                    for (int i = 0; i < 3; ++i)
                        m_points[i] = new Point(i, 1);
                    m_points[3] = new Point(2, 2);
                    break;
                case FigureType.L:
                    for (int i = 0; i < 3; ++i)
                        m_points[i] = new Point(i, 1);
                    m_points[3] = new Point(0, 2);
                    break;
            }
        }

        public void Move(int x, int y)
        {
            for (int i = 0; i < m_points.Length; ++i)
                m_points[i] = new Point(m_points[i].X + x, m_points[i].Y + y);
        }
        
        public Point[] Points { get { return m_points; } }
        public int Rotation { get { return m_rotation; } set { m_rotation = value; } }
        public FigureType Type { get { return m_type; } }

        Point[] m_points;
        int m_rotation;
        FigureType m_type;
    }

    public class Game
    {
        public Game(int fieldWidth, int fieldHeight)
        {
            m_score = 0;
            FieldWidth = fieldWidth;
            FieldHeight = fieldHeight;
            m_nextFigure = null;
            m_matrix = new sbyte[FieldHeight, FieldWidth];
            for (int y = 0; y < FieldHeight; ++y)
                for (int x = 0; x < FieldWidth; ++x)
                    m_matrix[y, x] = -1;
        }

        public bool Generate()
        {
            Random random = new Random();
            if (m_nextFigure == null)
                m_nextFigure = new Figure((FigureType)random.Next(0, 7));
            m_currentFigure = m_nextFigure;
            m_currentFigure.Move(FieldWidth / 2 - 1, -1);
            for (int i = 0; i < m_currentFigure.Points.Length; ++i)
            {
                int x = m_currentFigure.Points[i].X;
                int y = m_currentFigure.Points[i].Y;
                if (m_matrix[y, x] != -1)
                    return false;
            }
            m_nextFigure = new Figure((FigureType)random.Next(0, 7));
            return true;
        }

        public bool Rotate(Figure f)
        {
            Point[] def = new Point[f.Points.Length];
            Array.Copy(f.Points, def, f.Points.Length);
            Point[] pts = f.Points;
            int c;
            int rotation = f.Rotation;
            switch (f.Type)
            {
                case FigureType.I:
                    c = f.Rotation == 0 ? 1 : -1;
                    pts[0] = new Point(pts[0].X + c, pts[0].Y - c);
                    pts[2] = new Point(pts[2].X - c, pts[2].Y + c);
                    pts[3] = new Point(pts[3].X - 2 * c, pts[3].Y + 2 * c);
                    f.Rotation = f.Rotation == 0 ? 1 : 0;
                    break;
                case FigureType.T:
                    switch (f.Rotation)
                    {
                        case 0:
                            pts[0] = new Point(pts[0].X + 1, pts[0].Y - 1);
                            pts[2] = new Point(pts[2].X - 1, pts[2].Y + 1);
                            pts[3] = new Point(pts[3].X - 1, pts[3].Y - 1);
                            f.Rotation = 1;
                            break;
                        case 1:
                            pts[2] = new Point(pts[2].X + 1, pts[2].Y - 1);
                            f.Rotation = 2;
                            break;
                        case 2:
                            pts[3] = new Point(pts[3].X + 1, pts[3].Y + 1);
                            f.Rotation = 3;
                            break;
                        case 3:
                            pts[0] = new Point(pts[0].X - 1, pts[0].Y + 1);
                            f.Rotation = 0;
                            break;
                    }
                    break;
                case FigureType.S:
                    c = f.Rotation == 0 ? 1 : -1;
                    pts[0] = new Point(pts[0].X, pts[0].Y - c);
                    pts[2] = new Point(pts[2].X + c, pts[2].Y - c);
                    pts[3] = new Point(pts[3].X + c, pts[3].Y);
                    f.Rotation = f.Rotation == 0 ? 1 : 0;
                    break;
                case FigureType.Z:
                    c = f.Rotation == 0 ? 1 : -1;
                    pts[0] = new Point(pts[0].X + 2*c, pts[0].Y - c);
                    pts[3] = new Point(pts[3].X, pts[3].Y - c);
                    f.Rotation = f.Rotation == 0 ? 1 : 0;
                    break;
                case FigureType.J:
                    switch (f.Rotation)
                    {
                        case 0:
                            pts[0] = new Point(pts[0].X + 1, pts[0].Y - 1);
                            pts[2] = new Point(pts[2].X - 1, pts[2].Y + 1);
                            pts[3] = new Point(pts[3].X - 2, pts[3].Y);
                            f.Rotation = 1;
                            break;
                        case 1:
                            pts[0] = new Point(pts[0].X - 1, pts[0].Y);
                            pts[2] = new Point(pts[2].X + 1, pts[2].Y - 1);
                            pts[3] = new Point(pts[3].X, pts[3].Y - 1);
                            f.Rotation = 2;
                            break;
                        case 2:
                            pts[0] = new Point(pts[0].X + 1, pts[0].Y);
                            pts[2] = new Point(pts[2].X, pts[2].Y - 1);
                            pts[3] = new Point(pts[3].X + 1, pts[3].Y + 1);
                            f.Rotation = 3;
                            break;
                        case 3:
                            pts[0] = new Point(pts[0].X - 1, pts[0].Y + 1);
                            pts[2] = new Point(pts[2].X, pts[2].Y + 1);
                            pts[3] = new Point(pts[3].X + 1, pts[3].Y);
                            f.Rotation = 0;
                            break;
                    }
                    break;
                case FigureType.L:
                    switch (f.Rotation)
                    {
                        case 0:
                            pts[0] = new Point(pts[0].X + 1, pts[0].Y - 1);
                            pts[2] = new Point(pts[2].X - 1, pts[2].Y + 1);
                            pts[3] = new Point(pts[3].X, pts[3].Y - 2);
                            f.Rotation = 1;
                            break;
                        case 1:
                            pts[0] = new Point(pts[0].X + 1, pts[0].Y);
                            pts[2] = new Point(pts[2].X + 1, pts[2].Y - 1);
                            pts[3] = new Point(pts[3].X, pts[3].Y + 1);
                            f.Rotation = 2;
                            break;
                        case 2:
                            pts[0] = new Point(pts[0].X - 1, pts[0].Y);
                            pts[2] = new Point(pts[2].X - 1, pts[2].Y + 1);
                            pts[3] = new Point(pts[3].X + 2, pts[3].Y + 1);
                            f.Rotation = 3;
                            break;
                        case 3:
                            pts[0] = new Point(pts[0].X - 1, pts[0].Y + 1);
                            pts[2] = new Point(pts[2].X + 1, pts[2].Y - 1);
                            pts[3] = new Point(pts[3].X - 2, pts[3].Y);
                            f.Rotation = 0;
                            break;
                    }
                    break;
            }
            for (int i = 0; i < pts.Length; ++i)
            {
                int x = pts[i].X;
                int y = pts[i].Y;
                if (x < 0 || y < 0 || x >= FieldWidth || y >= FieldHeight || m_matrix[y, x] != -1)
                {
                    Array.Copy(def, f.Points, def.Length);
                    f.Rotation = rotation;
                    return false;
                }
            }
            return true;
        }

        public bool RemoveLine()
        {
            bool removed = false;
            int count = 0;
            for (int y = FieldHeight - 1; y >= 0; --y)
            {
                bool isFull = true;
                for (int x = 0; x < FieldWidth; ++x)
                    if (m_matrix[y, x] == -1)
                    {
                        isFull = false;
                        break;
                    }
                if (isFull == false)
                    continue;
                else
                {
                    removed = true;
                    count++;
                    for (int j = y - 1; j >= 0; --j)
                    {
                        for (int i = 0; i < FieldWidth; ++i)
                            m_matrix[j + 1, i] = m_matrix[j, i];
                    }
                    y++;
                }
            }
            m_score += count * 100;
            return removed;
        }

        public bool Fall()
        {
            if (m_currentFigure == null)
                return false;
            for (int i = m_currentFigure.Points.Length - 1; i >= 0; --i)
            {
                int x = m_currentFigure.Points[i].X;
                int y = m_currentFigure.Points[i].Y;
                if (y + 1 == FieldHeight || m_matrix[y + 1, x] != -1)
                {
                    for (int j = i + 1; j < m_currentFigure.Points.Length; ++j)
                    {
                        x = m_currentFigure.Points[j].X;
                        y = m_currentFigure.Points[j].Y;
                        m_currentFigure.Points[j] = new Point(x, y - 1);
                    }
                    for (int j = 0; j < m_currentFigure.Points.Length; ++j)
                    {
                        x = m_currentFigure.Points[j].X;
                        y = m_currentFigure.Points[j].Y;
                        m_matrix[y, x] = (sbyte)m_currentFigure.Type;
                    }
                    return false;
                }
                m_currentFigure.Points[i] = new Point(x, y + 1);
            }
            return true;
        }

        public bool Move(int dir)
        {
            if (m_currentFigure == null)
                return false;
            for (int i = 0; i < m_currentFigure.Points.Length; ++i)
            {
                int x = m_currentFigure.Points[i].X;
                int y = m_currentFigure.Points[i].Y;
                if (x + dir < 0 || x + dir >= FieldWidth || m_matrix[y, x + dir] != -1)
                {
                    for (int j = i - 1; j >= 0; --j)
                    {
                        x = m_currentFigure.Points[j].X;
                        y = m_currentFigure.Points[j].Y;
                        m_currentFigure.Points[j] = new Point(x - dir, y);
                    }
                    return false;
                }
                m_currentFigure.Points[i] = new Point(x + dir, y);
            }
            return true;
        }

        public sbyte GetValue(int x, int y)
        {
            return m_matrix[y, x];
        }

        //private Figure buildFigure(FigureType type)
        //{
        //    Figure figure = new Figure();
        //    figure.m_type = type;
        //    figure.m_points = new Point[4];
        //    Point[] pts = figure.m_points;
        //    Point p = new Point(FieldWidth / 2, 0);
        //    switch (type)
        //    {
        //        case FigureType.I:
        //            for (int i = 0; i < 4; ++i)
        //                pts[i] = new Point(p.X - 2 + i, 0);
        //            break;
        //        case FigureType.O:
        //            for (int i = 0; i < 2; ++i)
        //                for (int j = 0; j < 2; ++j)
        //                    pts[i * 2 + j] = new Point(p.X - 1 + j, i);
        //            break;
        //        case FigureType.T:
        //            for (int i = 0; i < 3; ++i)
        //                    pts[i] = new Point(p.X - 1 + i, 0);
        //            pts[3] = new Point(p.X, 1);
        //            break;
        //        case FigureType.S:
        //            for (int i = 0; i < 2; ++i)
        //                pts[i] = new Point(p.X + i, 0);
        //            for (int i = 0; i < 2; ++i)
        //                pts[i + 2] = new Point(p.X - 1 + i, 1);
        //            break;
        //        case FigureType.Z:
        //            for (int i = 0; i < 2; ++i)
        //                    pts[i] = new Point(p.X - 1 + i, 0);
        //            for (int i = 0; i < 2; ++i)
        //                pts[i + 2] = new Point(p.X + i, 1);
        //            break;
        //        case FigureType.J:
        //            for (int i = 0; i < 3; ++i)
        //                pts[i] = new Point(p.X - 1 + i, 0);
        //            pts[3] = new Point(p.X + 1, 1);
        //            break;
        //        case FigureType.L:
        //            for (int i = 0; i < 3; ++i)
        //                pts[i] = new Point(p.X - 1 + i, 0);
        //            pts[3] = new Point(p.X - 1, 1);
        //            break;
        //        default:
        //            return null;
        //    }
        //    for (int i = 0; i < pts.Length; ++i)
        //    {
        //        int x = pts[i].X;
        //        int y = pts[i].Y;
        //        if (m_matrix[y, x] != -1)
        //            return null;
        //    }
        //    return figure;
        //}

        public readonly int FieldWidth;
        public readonly int FieldHeight;
        public Figure NextFigure { get { return m_nextFigure; } }
        public Figure CurrentFigure { get { return m_currentFigure; } }
        public int Score { get { return m_score; } }

        Figure m_currentFigure;
        Figure m_nextFigure;
        sbyte[,] m_matrix;
        int m_score;
    }
}
