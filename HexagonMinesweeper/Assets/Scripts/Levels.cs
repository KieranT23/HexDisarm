﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All the levels
/// </summary>
public static class Levels
{
    public static Dictionary<int, List<int>> AllLevels = new Dictionary<int, List<int>>()
    {
        //Level, {gridSize, bombs, distance from bomb to show (0 = don't show anything), seed}
        {1, new List<int> {3, 1, 1, 11}},
        {2, new List<int> {4, 1, 0, 3}},
        {3, new List<int> {4, 2, 0, 10, 4}},
        {4, new List<int> {4, 1, 1, 308}},
        {5, new List<int> {4, 1, 1, 3}},
        {6, new List<int> {5, 2, 2, 77, 739}},
        {7, new List<int> {5, 2, 2, 310, 21}},
        {8, new List<int> {5, 2, 2, 574, 456}},
        {9, new List<int> {5, 2, 3, 10, 173}},
        {10, new List<int> {5, 2, 3, 644, 83}},
        ///////////////////////////////////////////////
        {11, new List<int> {6, 2, 3, 642, 754}},
        {12, new List<int> {6, 2, 3, 848, 19}},
        {13, new List<int> {6, 2, 0, 680, 66}},
        {14, new List<int> {6, 2, 0, 308, 769}},
        {15, new List<int> {6, 2, 0, 831, 105}},
        {16, new List<int> {6, 2, 0, 692, 739}},
        {17, new List<int> {6, 2, 0, 310, 21}},
        {18, new List<int> {6, 2, 0, 814, 944}},
        {19, new List<int> {6, 2, 0, 565, 456}},
        {20, new List<int> {6, 2, 0, 644, 85}},
        //////////////////////////////////////////////////
        {21, new List<int> {6, 2, 0, 221, 13}},
        {22, new List<int> {6, 2, 0, 530, 739}},
        {23, new List<int> {6, 2, 0, 196, 927}},
        {24, new List<int> {6, 2, 0, 83, 565}},
        {25, new List<int> {6, 3, 0, 999, 447, 2}},
        {26, new List<int> {6, 3, 0, 38, 1, 12}},
        {27, new List<int> {6, 3, 0, 2, 964, 197}},
        {28, new List<int> {6, 4, 0, 291, 113, 341, 817}},
        {29, new List<int> {6, 4, 0, 648, 754, 286, 869}},
        {30, new List<int> {6, 4, 0, 644, 83, 578, 291, 64}},
        /////////////////////////////////////////////////////
        {31, new List<int> {7, 3, 0, 319, 719, 213}},
        {32, new List<int> {7, 3, 0, 822, 742, 581}},
        {33, new List<int> {7, 3, 0, 701, 215, 37}},
        {34, new List<int> {7, 3, 0, 254, 936, 511}},
        {35, new List<int> {7, 4, 0, 188, 36, 390, 331}},
        {36, new List<int> {7, 4, 0, 284, 180, 95, 464}},
        {37, new List<int> {7, 4, 0, 232, 569, 498, 84}},
        {38, new List<int> {7, 5, 0, 413, 472, 215, 17, 877}},
        {39, new List<int> {7, 5, 0, 344, 830, 137, 668, 672}},
        {40, new List<int> {7, 5, 0, 783, 447, 767, 902, 236}},
        ///////////////////////////////////////////////
        {41, new List<int> {8, 2, 0, 289, 968}},
        {42, new List<int> {8, 4, 0, 897, 304, 959, 491}},
        {43, new List<int> {8, 4, 0, 69, 994, 323, 742}},
        {44, new List<int> {8, 5, 0, 308, 992, 19, 989, 168}},
        {45, new List<int> {8, 5, 0, 515, 700, 117, 493, 869}},
        {46, new List<int> {8, 6, 0, 369, 225, 647, 530, 618, 230}},
        {47, new List<int> {8, 6, 0, 592, 60, 715, 302, 270, 565}},
        {48, new List<int> {8, 6, 0, 119, 862, 690, 84, 5, 754}},
        {49, new List<int> {8, 6, 0, 102, 364, 164, 31, 873, 299}},
        {50, new List<int> {8, 6, 0, 735, 19, 98, 205, 416, 697}},
        ///////////////////////////////////////////////////
        {51, new List<int> {8, 6, 0, 511, 512, 513, 514, 515, 516}},
        {52, new List<int> {8, 6, 0, 517, 518, 519, 520, 521, 522}},
        {53, new List<int> {8, 6, 0, 523, 524, 525, 526, 527, 528}},
        {54, new List<int> {8, 6, 0, 529, 530, 531, 532, 533, 534}},
        {55, new List<int> {8, 6, 0, 535, 536, 537, 538, 539, 540}},
        {56, new List<int> {8, 6, 0, 541, 542, 543, 544, 545, 546}},
        {57, new List<int> {8, 6, 0, 547, 548, 549, 550, 551, 552}},
        {58, new List<int> {8, 6, 0, 553, 554, 555, 556, 557, 558}},
        {59, new List<int> {8, 6, 0, 559, 560, 561, 562, 563, 564}},
        {60, new List<int> {8, 6, 0, 565, 566, 567, 568, 569, 570}},
        ///////////////////////////////////////////////
        {61, new List<int> {8, 6, 0, 611, 612, 613, 614, 615, 616}},
        {62, new List<int> {8, 6, 0, 617, 618, 619, 620, 621, 622}},
        {63, new List<int> {8, 6, 0, 623, 624, 625, 626, 627, 628}},
        {64, new List<int> {8, 6, 0, 629, 630, 631, 632, 633, 634}},
        {65, new List<int> {8, 6, 0, 635, 636, 637, 638, 639, 640}},
        {66, new List<int> {8, 6, 0, 641, 642, 643, 644, 645, 646}},
        {67, new List<int> {8, 6, 0, 647, 648, 649, 650, 651, 652}},
        {68, new List<int> {8, 6, 0, 653, 654, 655, 656, 657, 658}},
        {69, new List<int> {8, 6, 0, 659, 660, 661, 662, 663, 664}},
        {70, new List<int> {8, 6, 0, 665, 666, 667, 668, 669, 670}},
        //////////////////////////////////////////////////
        {71, new List<int> {7, 6, 0, 711, 712, 713, 714, 715, 716}},
        {72, new List<int> {7, 6, 0, 717, 718, 719, 720, 721, 722}},
        {73, new List<int> {7, 6, 0, 723, 724, 725, 726, 727, 728}},
        {74, new List<int> {7, 6, 0, 729, 730, 731, 732, 733, 734}},
        {75, new List<int> {7, 6, 0, 735, 736, 737, 738, 739, 740}},
        {76, new List<int> {7, 6, 0, 741, 742, 743, 744, 745, 746}},
        {77, new List<int> {7, 6, 0, 747, 748, 749, 750, 751, 752}},
        {78, new List<int> {7, 6, 0, 753, 754, 755, 756, 757, 758}},
        {79, new List<int> {7, 6, 0, 759, 760, 761, 762, 763, 764}},
        {80, new List<int> {7, 6, 0, 765, 766, 767, 768, 769, 770}},
        /////////////////////////////////////////////////////
        {81, new List<int> {7, 6, 0, 811, 812, 813, 814, 815, 816}},
        {82, new List<int> {7, 6, 0, 817, 818, 819, 820, 821, 822}},
        {83, new List<int> {7, 6, 0, 823, 824, 825, 826, 827, 828}},
        {84, new List<int> {7, 6, 0, 829, 830, 831, 832, 833, 834}},
        {85, new List<int> {7, 6, 0, 835, 836, 837, 838, 839, 840}},
        {86, new List<int> {7, 6, 0, 841, 842, 843, 844, 845, 846}},
        {87, new List<int> {7, 6, 0, 847, 848, 849, 850, 851, 852}},
        {88, new List<int> {7, 6, 0, 853, 854, 855, 856, 857, 858}},
        {89, new List<int> {7, 6, 0, 859, 860, 861, 862, 863, 864}},
        {90, new List<int> {7, 6, 0, 865, 866, 867, 868, 869, 870}},
        ///////////////////////////////////////////////
        {91, new List<int> {8, 7, 0, 911, 912, 913, 914, 915, 916, 116}},
        {92, new List<int> {8, 7, 0, 917, 918, 919, 920, 921, 922, 122}},
        {93, new List<int> {8, 7, 0, 923, 924, 925, 926, 927, 928, 128}},
        {94, new List<int> {8, 7, 0, 929, 930, 931, 932, 933, 934, 134}},
        {95, new List<int> {8, 7, 0, 935, 936, 937, 938, 939, 940, 140}},
        {96, new List<int> {8, 7, 0, 941, 942, 943, 944, 945, 946, 146}},
        {97, new List<int> {8, 7, 0, 947, 948, 949, 950, 951, 952, 152}},
        {98, new List<int> {8, 7, 0, 953, 954, 955, 956, 957, 958, 158}},
        {99, new List<int> {8, 7, 0, 959, 960, 961, 962, 963, 964, 164}},
        {100, new List<int> {8, 7, 0, 965, 966, 967, 968, 969, 970, 170}},
        ///////////////////////////////////////////////////
        {101, new List<int> {8, 7, 0, 1011, 1012, 1013, 1014, 1015, 1016, 216}},
        {102, new List<int> {8, 7, 0, 1017, 1018, 1019, 1020, 1021, 1022, 222}},
        {103, new List<int> {8, 7, 0, 1023, 1024, 1025, 1026, 1027, 1028, 228}},
        {104, new List<int> {8, 7, 0, 1029, 1030, 1031, 1032, 1033, 1034, 234}},
        {105, new List<int> {8, 7, 0, 1035, 1036, 1037, 1038, 1039, 1040, 240}},
        {106, new List<int> {8, 7, 0, 1041, 1042, 1043, 1044, 1045, 1046, 246}},
        {107, new List<int> {8, 7, 0, 1047, 1048, 1049, 1050, 1051, 1052, 252}},
        {108, new List<int> {8, 7, 0, 1053, 1054, 1055, 1056, 1057, 1058, 258}},
        {109, new List<int> {8, 7, 0, 1059, 1060, 1061, 1062, 1063, 1064, 264}},
        {110, new List<int> {8, 7, 0, 1065, 1066, 1067, 1068, 1069, 1070, 270}},
        ///////////////////////////////////////////////
        {111, new List<int> {7, 7, 0, 1111, 1112, 1113, 1114, 1115, 1116, 316}},
        {112, new List<int> {7, 7, 0, 1117, 1118, 1119, 1120, 1121, 1122, 322}},
        {113, new List<int> {7, 7, 0, 1123, 1124, 1125, 1126, 1127, 1128, 328}},
        {114, new List<int> {7, 7, 0, 1129, 1130, 1131, 1132, 1133, 1134, 334}},
        {115, new List<int> {7, 7, 0, 1135, 1136, 1137, 1138, 1139, 1140, 340}},
        {116, new List<int> {7, 7, 0, 1141, 1142, 1143, 1144, 1145, 1146, 346}},
        {117, new List<int> {7, 7, 0, 1147, 1148, 1149, 1150, 1151, 1152, 352}},
        {118, new List<int> {7, 7, 0, 1153, 1154, 1155, 1156, 1157, 1158, 358}},
        {119, new List<int> {7, 7, 0, 1159, 1160, 1161, 1162, 1163, 1164, 364}},
        {120, new List<int> {7, 7, 0, 1165, 1166, 1167, 1168, 1169, 1170, 370}},
        //////////////////////////////////////////////////
        {121, new List<int> {7, 7, 0, 1211, 1212, 1213, 1214, 1215, 1216, 416}},
        {122, new List<int> {7, 7, 0, 1217, 1218, 1219, 1220, 1221, 1222, 422}},
        {123, new List<int> {7, 7, 0, 1223, 1224, 1225, 1226, 1227, 1228, 428}},
        {124, new List<int> {7, 7, 0, 1229, 1230, 1231, 1232, 1233, 1234, 434}},
        {125, new List<int> {7, 7, 0, 1235, 1236, 1237, 1238, 1239, 1240, 440}},
        {126, new List<int> {7, 7, 0, 1241, 1242, 1243, 1244, 1245, 1246, 446}},
        {127, new List<int> {7, 7, 0, 1247, 1248, 1249, 1250, 1251, 1252, 452}},
        {128, new List<int> {7, 7, 0, 1253, 1254, 1255, 1256, 1257, 1258, 458}},
        {129, new List<int> {7, 7, 0, 1259, 1260, 1261, 1262, 1263, 1264, 464}},
        {130, new List<int> {7, 7, 0, 1265, 1266, 1267, 1268, 1269, 1270, 470}},
        /////////////////////////////////////////////////////
        {131, new List<int> {6, 7, 0, 1311, 1312, 1313, 1314, 1315, 1316, 1016}},
        {132, new List<int> {6, 7, 0, 1317, 1318, 1319, 1320, 1321, 1322, 1022}},
        {133, new List<int> {6, 7, 0, 1323, 1324, 1325, 1326, 1327, 1328, 1028}},
        {134, new List<int> {6, 7, 0, 1329, 1330, 1331, 1332, 1333, 1334, 1034}},
        {135, new List<int> {6, 7, 0, 1335, 1336, 1337, 1338, 1339, 1340, 1040}},
        {136, new List<int> {6, 7, 0, 1341, 1342, 1343, 1344, 1345, 1346, 1046}},
        {137, new List<int> {6, 7, 0, 1347, 1348, 1349, 1350, 1351, 1352, 1052}},
        {138, new List<int> {6, 7, 0, 1353, 1354, 1355, 1356, 1357, 1358, 1058}},
        {139, new List<int> {6, 7, 0, 1359, 1360, 1361, 1362, 1363, 1364, 1064}},
        {140, new List<int> {6, 7, 0, 1365, 1366, 1367, 1368, 1369, 1370, 1070}},
        ///////////////////////////////////////////////
        {141, new List<int> {6, 7, 0, 1411, 1412, 1413, 1414, 1415, 1416, 1116}},
        {142, new List<int> {6, 7, 0, 1417, 1418, 1419, 1420, 1421, 1422, 1122}},
        {143, new List<int> {6, 7, 0, 1423, 1424, 1425, 1426, 1427, 1428, 1128}},
        {144, new List<int> {6, 7, 0, 1429, 1430, 1431, 1432, 1433, 1434, 1134}},
        {145, new List<int> {6, 7, 0, 1435, 1436, 1437, 1438, 1439, 1440, 1140}},
        {146, new List<int> {6, 7, 0, 1441, 1442, 1443, 1444, 1445, 1446, 1146}},
        {147, new List<int> {6, 7, 0, 1447, 1448, 1449, 1450, 1451, 1452, 1152}},
        {148, new List<int> {6, 7, 0, 1453, 1454, 1455, 1456, 1457, 1458, 1158}},
        {149, new List<int> {6, 7, 0, 1459, 1460, 1461, 1462, 1463, 1464, 1164}},
        {150, new List<int> {6, 7, 0, 1465, 1466, 1467, 1468, 1469, 1470, 1170}},
        ///////////////////////////////////////////////////
        {151, new List<int> {8, 8, 0, 1511, 1512, 1513, 1514, 1515, 1516, 1216, 11216}},
        {152, new List<int> {8, 8, 0, 1517, 1518, 1519, 1520, 1521, 1522, 1222, 11222}},
        {153, new List<int> {8, 8, 0, 1523, 1524, 1525, 1526, 1527, 1528, 1228, 11228}},
        {154, new List<int> {8, 8, 0, 1529, 1530, 1531, 1532, 1533, 1534, 1234, 11234}},
        {155, new List<int> {8, 8, 0, 1535, 1536, 1537, 1538, 1539, 1540, 1240, 11240}},
        {156, new List<int> {8, 8, 0, 1541, 1542, 1543, 1544, 1545, 1546, 1246, 11246}},
        {157, new List<int> {8, 8, 0, 1547, 1548, 1549, 1550, 1551, 1552, 1252, 11252}},
        {158, new List<int> {8, 8, 0, 1553, 1554, 1555, 1556, 1557, 1558, 1258, 11258}},
        {159, new List<int> {8, 8, 0, 1559, 1560, 1561, 1562, 1563, 1564, 1264, 11264}},
        {160, new List<int> {8, 8, 0, 1565, 1566, 1567, 1568, 1569, 1570, 1270, 11270}},
        ///////////////////////////////////////////////
        {161, new List<int> {7, 8, 0, 1611, 1612, 1613, 1614, 1615, 1616, 1316, 21216}},
        {162, new List<int> {7, 8, 0, 1617, 1618, 1619, 1620, 1621, 1622, 1322, 21222}},
        {163, new List<int> {7, 8, 0, 1623, 1624, 1625, 1626, 1627, 1628, 1328, 21228}},
        {164, new List<int> {7, 8, 0, 1629, 1630, 1631, 1632, 1633, 1634, 1334, 21234}},
        {165, new List<int> {7, 8, 0, 1635, 1636, 1637, 1638, 1639, 1640, 1340, 21240}},
        {166, new List<int> {7, 8, 0, 1641, 1642, 1643, 1644, 1645, 1646, 1346, 21246}},
        {167, new List<int> {7, 8, 0, 1647, 1648, 1649, 1650, 1651, 1652, 1352, 21252}},
        {168, new List<int> {7, 8, 0, 1653, 1654, 1655, 1656, 1657, 1658, 1358, 21258}},
        {169, new List<int> {7, 8, 0, 1659, 1660, 1661, 1662, 1663, 1664, 1364, 21264}},
        {170, new List<int> {7, 8, 0, 1665, 1666, 1667, 1668, 1669, 1670, 1370, 21270}},
        //////////////////////////////////////////////////
        {171, new List<int> {7, 8, 0, 1711, 1712, 1713, 1714, 1715, 1716, 1416, 31216}},
        {172, new List<int> {7, 8, 0, 1717, 1718, 1719, 1720, 1721, 1722, 1422, 31222}},
        {173, new List<int> {7, 8, 0, 1723, 1724, 1725, 1726, 1727, 1728, 1428, 31228}},
        {174, new List<int> {7, 8, 0, 1729, 1730, 1731, 1732, 1733, 1734, 1434, 31234}},
        {175, new List<int> {7, 8, 0, 1735, 1736, 1737, 1738, 1739, 1740, 1440, 31240}},
        {176, new List<int> {7, 8, 0, 1741, 1742, 1743, 1744, 1745, 1746, 1446, 31246}},
        {177, new List<int> {7, 8, 0, 1747, 1748, 1749, 1750, 1751, 1752, 1452, 31252}},
        {178, new List<int> {7, 8, 0, 1753, 1754, 1755, 1756, 1757, 1758, 1458, 31258}},
        {179, new List<int> {7, 8, 0, 1759, 1760, 1761, 1762, 1763, 1764, 1464, 31264}},
        {180, new List<int> {7, 8, 0, 1765, 1766, 1767, 1768, 1769, 1770, 1470, 31270}},
        /////////////////////////////////////////////////////
        {181, new List<int> {6, 8, 0, 1811, 1812, 1813, 1814, 1815, 1816, 1516, 51216}},
        {182, new List<int> {6, 8, 0, 1817, 1818, 1819, 1820, 1821, 1822, 1522, 51222}},
        {183, new List<int> {6, 8, 0, 1823, 1824, 1825, 1826, 1827, 1828, 1528, 51228}},
        {184, new List<int> {6, 8, 0, 1829, 1830, 1831, 1832, 1833, 1834, 1534, 51234}},
        {185, new List<int> {6, 8, 0, 1835, 1836, 1837, 1838, 1839, 1840, 1540, 51240}},
        {186, new List<int> {6, 8, 0, 1841, 1842, 1843, 1844, 1845, 1846, 1546, 51246}},
        {187, new List<int> {6, 8, 0, 1847, 1848, 1849, 1850, 1851, 1852, 1552, 51252}},
        {188, new List<int> {6, 8, 0, 1853, 1854, 1855, 1856, 1857, 1858, 1558, 51258}},
        {189, new List<int> {6, 8, 0, 1859, 1860, 1861, 1862, 1863, 1864, 1564, 51264}},
        {190, new List<int> {6, 8, 0, 1865, 1866, 1867, 1868, 1869, 1870, 1570, 51270}},
        ///////////////////////////////////////////////
        {191, new List<int> {6, 8, 0, 1911, 1912, 1913, 1914, 1915, 1916, 1916, 61216}},
        {192, new List<int> {6, 8, 0, 1917, 1918, 1919, 1920, 1921, 1922, 1922, 61222}},
        {193, new List<int> {6, 8, 0, 1923, 1924, 1925, 1926, 1927, 1928, 1928, 61228}},
        {194, new List<int> {6, 8, 0, 1929, 1930, 1931, 1932, 1933, 1934, 1934, 61234}},
        {195, new List<int> {6, 8, 0, 1935, 1936, 1937, 1938, 1939, 1940, 1940, 61240}},
        {196, new List<int> {6, 8, 0, 1941, 1942, 1943, 1944, 1945, 1946, 1946, 61246}},
        {197, new List<int> {6, 8, 0, 1947, 1948, 1949, 1950, 1951, 1952, 1952, 61252}},
        {198, new List<int> {6, 8, 0, 1953, 1954, 1955, 1956, 1957, 1958, 1958, 61258}},
        {199, new List<int> {6, 8, 0, 1959, 1960, 1961, 1962, 1963, 1964, 1964, 61264}},
        {200, new List<int> {6, 8, 0, 1965, 1966, 1967, 1968, 1969, 1970, 1970, 61270}},
        ///////////////////////////////////////////////////
    };

}
