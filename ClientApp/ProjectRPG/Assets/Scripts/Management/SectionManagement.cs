using GameCore;
using System.Collections.Generic;

public class SectionManagement : Singleton<SectionManagement>
{
    /// <summary>
    /// Around session dictionary.
    /// key: Section.
    /// value: Around sections.
    /// </summary>
    private Dictionary<(int sectionX, int sectionY), HashSet<(int, int)>> _aroundSessionDict;

    private void Start()
    {
        var confArea = TDBManagement.Instance.First<TB_Area>(d => d.Area.Equals(Type_Area.HuntZone));

        var sectionMinX = confArea.MinX / GameData.SECTION_WIDTH;
        var sectionMaxX = confArea.MaxX / GameData.SECTION_WIDTH;

        var sectionMinY = confArea.MinY / GameData.SECTION_HEIGHT;
        var sectionMaxY = confArea.MaxY / GameData.SECTION_HEIGHT;

        _aroundSessionDict = new Dictionary<(int sectionX, int sectionY), HashSet<(int, int)>>();

        for (var sectionX = sectionMinX; sectionX <= sectionMaxX; ++sectionX)
        {
            for (var sectionY = sectionMinY; sectionY <= sectionMaxY; ++sectionY)
            {
                var section = (sectionX, sectionY);

                _aroundSessionDict.Add(section, new HashSet<(int, int)>());
            }
        }

        // ��ó ���� ���.
        foreach (var pair in _aroundSessionDict)
        {
            var aroundSections = pair.Value;
            var sectionX = pair.Key.sectionX;
            var sectionY = pair.Key.sectionY;

            var calcAroundSections = Calc_AroundSections(sectionMinX, sectionMaxX, sectionMinY, sectionMaxY, sectionX, sectionY);

            foreach (var calcSectoin in calcAroundSections)
            {
                aroundSections.Add(calcSectoin);
            }
        }
    }

    /// <summary>
    /// Check two section are around.
    /// </summary>
    /// <param name="target">Target section</param>
    /// <param name="comparer">Comparer section</param>
    /// <returns>true: comparer is around to target. false: comparer is not around to target</returns>
    public bool IsAroundSection((int sectionX, int sectionY) target, (int sectionX, int sectionY) comparer)
    {
        // Comparer.
        if (false == _aroundSessionDict.ContainsKey(comparer))
        {
            return false;
        }

        // Target.
        if (false == _aroundSessionDict.TryGetValue(target, out var sections))
        {
            return false;
        }

        return sections.Contains(comparer);
    }

    /// <summary>
    /// Calculate around sections.
    /// </summary>
    private List<(int, int)> Calc_AroundSections(int sectionMinX, int sectionMaxX, int sectionMinY, int sectionMaxY, int sectionX, int sectionY)
    {
        var aroundSections = new List<(int, int)>();

        // ���� ���� �߰�.
        aroundSections.Add((sectionX, sectionY));

        ///////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////
        // Top.
        var topSection = (sectionX, sectionY + 1);
        if (topSection.Item2 <= sectionMaxY)
        {
            aroundSections.Add(topSection);
        }

        // Down.
        var downSection = (sectionX, sectionY - 1);
        if (downSection.Item2 >= sectionMinY)
        {
            aroundSections.Add(downSection);
        }

        // Left.
        var leftSection = (sectionX - 1, sectionY);
        if (leftSection.Item1 >= sectionMinX)
        {
            aroundSections.Add(leftSection);
        }

        // Right.
        var rightSection = (sectionX + 1, sectionY);
        if (rightSection.Item1 <= sectionMaxX)
        {
            aroundSections.Add(rightSection);
        }

        ///////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////
        // LeftTop.
        var leftTopSection = (sectionX - 1, sectionY + 1);
        if (leftTopSection.Item1 >= sectionMinX && leftTopSection.Item2 <= sectionMaxY)
        {
            aroundSections.Add(leftTopSection);
        }

        // LeftDown.
        var leftDownSection = (sectionX - 1, sectionY - 1);
        if (leftDownSection.Item1 >= sectionMinX && leftDownSection.Item2 >= sectionMinY)
        {
            aroundSections.Add(leftDownSection);
        }

        // RightTop.
        var rightTopSection = (sectionX + 1, sectionY + 1);
        if (rightTopSection.Item1 <= sectionMaxX && rightTopSection.Item2 <= sectionMaxY)
        {
            aroundSections.Add(rightTopSection);
        }

        // RightDown.
        var rightDownSection = (sectionX + 1, sectionY - 1);
        if (rightDownSection.Item1 <= sectionMaxX && rightDownSection.Item2 >= sectionMinY)
        {
            aroundSections.Add(rightDownSection);
        }

        ///////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////
        // LeftTopEdge.
        var leftTopEdgeSection = (sectionX - 2, sectionY + 2);
        if (leftTopEdgeSection.Item1 >= sectionMinX && leftTopEdgeSection.Item2 <= sectionMaxY)
        {
            aroundSections.Add(leftTopEdgeSection);
        }

        // LeftTopTop.
        var leftTopTopSection = (sectionX - 1, sectionY + 2);
        if (leftTopTopSection.Item1 >= sectionMinX && leftTopTopSection.Item2 <= sectionMaxY)
        {
            aroundSections.Add(leftTopTopSection);
        }

        // TopTop.
        var topTopSection = (sectionX, sectionY + 2);
        if (topTopSection.Item2 <= sectionMaxY)
        {
            aroundSections.Add(topTopSection);
        }

        // RightTopTop.
        var rightTopTopSection = (sectionX + 1, sectionY + 2);
        if (rightTopTopSection.Item1 <= sectionMaxX && rightTopTopSection.Item2 <= sectionMaxY)
        {
            aroundSections.Add(rightTopTopSection);
        }

        // RightTopEdge.
        var rightTopEdgeSection = (sectionX + 2, sectionY + 2);
        if (rightTopEdgeSection.Item1 <= sectionMaxX && rightTopEdgeSection.Item2 <= sectionMaxY)
        {
            aroundSections.Add(rightTopEdgeSection);
        }

        // RightRightTop.
        var rightRightTopSection = (sectionX + 2, sectionY + 1);
        if (rightRightTopSection.Item1 <= sectionMaxX && rightRightTopSection.Item2 <= sectionMaxY)
        {
            aroundSections.Add(rightRightTopSection);
        }

        // RightRight.
        var rightRightSection = (sectionX + 2, sectionY);
        if (rightRightSection.Item1 <= sectionMaxX)
        {
            aroundSections.Add(rightRightSection);
        }

        // RightRightDown.
        var rightRightDownSection = (sectionX + 2, sectionY - 1);
        if (rightRightDownSection.Item1 <= sectionMaxX && rightRightDownSection.Item2 >= sectionMinY)
        {
            aroundSections.Add(rightRightDownSection);
        }

        // LeftLeftTop.
        var leftLeftTopSection = (sectionX - 2, sectionY + 1);
        if (leftLeftTopSection.Item1 >= sectionMinX && leftLeftTopSection.Item2 <= sectionMaxY)
        {
            aroundSections.Add(leftLeftTopSection);
        }

        // LeftLeft.
        var leftLeftSection = (sectionX - 2, sectionY);
        if (leftLeftSection.Item1 >= sectionMinX)
        {
            aroundSections.Add(leftLeftSection);
        }

        // LeftLeftDown.
        var leftLeftDownSection = (sectionX - 2, sectionY - 1);
        if (leftLeftDownSection.Item1 >= sectionMinX && leftLeftDownSection.Item2 >= sectionMinY)
        {
            aroundSections.Add(leftLeftDownSection);
        }

        // LeftDownEdge.
        var leftDownEdgeSection = (sectionX - 2, sectionY - 2);
        if (leftDownEdgeSection.Item1 >= sectionMinX && leftDownEdgeSection.Item2 >= sectionMinY)
        {
            aroundSections.Add(leftDownEdgeSection);
        }

        // LeftDownDown.
        var leftDownDownSection = (sectionX - 1, sectionY - 2);
        if (leftDownDownSection.Item1 >= sectionMinX && leftDownDownSection.Item2 >= sectionMinY)
        {
            aroundSections.Add(leftDownDownSection);
        }

        // DownDown.
        var downDownSection = (sectionX , sectionY - 2);
        if (downDownSection.Item2 >= sectionMinY)
        {
            aroundSections.Add(downDownSection);
        }

        // RightDownDown.
        var rightDownDownSection = (sectionX + 1, sectionY - 2);
        if (rightDownDownSection.Item1 <= sectionMaxX && rightDownDownSection.Item2 >= sectionMinY)
        {
            aroundSections.Add(rightDownDownSection);
        }

        // RightDownEdge.
        var rightDownEdgeSection = (sectionX + 2, sectionY - 2);
        if (rightDownEdgeSection.Item1 <= sectionMaxX && rightDownEdgeSection.Item2 >= sectionMinY)
        {
            aroundSections.Add(rightDownEdgeSection);
        }

        return aroundSections;
    }
}
