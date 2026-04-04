using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace AT.Toolbox
{
  /// <summary>
  /// Граф, представленный в виде списка смежных вершин
  /// </summary>
  /// <typeparam name="TNode">Тип вершины графа, используемый как ключ вершины</typeparam>
  /// <typeparam name="TEdge">Тип веса ребра графа</typeparam>
  public class AdjacencyList<TNode, TEdge>
  {
    private readonly bool m_oriented;
    private readonly LockSource m_lock = new LockSource();
    private readonly Dictionary<TNode, Dictionary<TNode, TEdge>> m_data = new Dictionary<TNode, Dictionary<TNode, TEdge>>();
    private HashSet<EdgeInfo<TNode, TEdge>> m_edges;

    private static readonly ILog _log = LogManager.GetLogger(string.Format("AdjacencyList<{0}, {1}>",
      typeof(TNode).Name, typeof(TEdge).Name));

    /// <summary>
    /// Инициализация нового пустого графа
    /// </summary>
    /// <param name="oriented">Является ли граф ориентированным</param>
    public AdjacencyList(bool oriented)
    {
      m_oriented = oriented;
    }

    /// <summary>
    /// Показывает, является ли граф ориентированным
    /// </summary>
    public bool Oriented
    {
      get { return m_oriented; }
    }

    /// <summary>
    /// Множество вершин
    /// </summary>
    public ICollection<TNode> Nodes
    {
      get { return m_data.Keys; }
    }

    /// <summary>
    /// Множество рёбер
    /// </summary>
    public HashSet<EdgeInfo<TNode, TEdge>> Edges
    {
      get
      {
        using (m_lock.GetReadLock())
        {
          if (m_edges != null)
            return m_edges;
        }

        using (m_lock.GetWriteLock())
        {
          if (m_edges != null)
            return m_edges;

          var ret = new HashSet<EdgeInfo<TNode, TEdge>>();

          foreach (var kv in m_data)
          {
            foreach (var ed in kv.Value)
            {
              var addee = new EdgeInfo<TNode, TEdge>(kv.Key, ed.Key, ed.Value);

              // Для неориентированного графа, если дуга является обращением
              // уже добавленной дуги, новую не добавляем
              if (!m_oriented)
              {
                if (ret.Contains(addee.Invert()))
                  continue;
              }

              ret.Add(addee);
            }
          }

          m_edges = ret;
          return m_edges;
        }
      }
    }

    public TEdge this[TNode fromNode, TNode toNode]
    {
      get
      {
        using (m_lock.GetReadLock())
        {
          return m_data[fromNode][toNode];
        }
      }
      set
      {
        this.SetEdge(fromNode, toNode, value);
      }
    }

    private void FindAdjacency(TNode fromNode, TNode toNode, out Dictionary<TNode, TEdge> from_edges, out Dictionary<TNode, TEdge> to_edges)
    {
      if (fromNode == null)
        throw new ArgumentNullException("fromNode");
      if (toNode == null)
        throw new ArgumentNullException("toNode");

      if (!m_data.TryGetValue(fromNode, out from_edges) || !m_data.TryGetValue(toNode, out to_edges))
        throw new ArgumentException("Graph node not found");
    }

    /// <summary>
    /// Добавление новой вершины, не связанной дугами
    /// </summary>
    /// <param name="node">Вершина</param>
    public void AddNode(TNode node)
    {
      if (node == null)
        throw new ArgumentNullException("node");

      using (m_lock.GetWriteLock())
      {
        m_data.Add(node, new Dictionary<TNode, TEdge>());

        m_edges = null;
      }
    }

    /// <summary>
    /// Удаление вершины и всех связанных с ней дуг
    /// </summary>
    /// <param name="node">Удаляемая вершина</param>
    /// <returns>True, если вершина была удалена. False, если вершина не была найдена</returns>
    public bool RemoveNode(TNode node)
    {
      using (m_lock.GetWriteLock())
      {
        if (m_data.Remove(node))
        {
          foreach (var kv in m_data)
            kv.Value.Remove(node);

          m_edges = null;
          return true;
        }
        else
          return false;
      }
    }

    /// <summary>
    /// Добавление дуги. Если дуга, соединяющая указанную пару вершин уже существует, возникает исключение
    /// </summary>
    /// <param name="fromNode">Начальная вершина в дуге</param>
    /// <param name="toNode">Конечная вершина в дуге</param>
    /// <param name="edge">Вес создаваемой дуги</param>
    public void AddEdge(TNode fromNode, TNode toNode, TEdge edge)
    {
      using (m_lock.GetWriteLock())
      {
        Dictionary<TNode, TEdge> from_edges;
        Dictionary<TNode, TEdge> to_edges;
        this.FindAdjacency(fromNode, toNode, out from_edges, out to_edges);

        from_edges.Add(toNode, edge);

        if (!m_oriented)
          to_edges.Add(fromNode, edge);

        m_edges = null;
      }
    }

    /// <summary>
    /// Добавление дуги
    /// </summary>
    /// <param name="edge">Сведения о дуге</param>
    public void AddEdge(EdgeInfo<TNode, TEdge> edge)
    {
      if (edge == null)
        throw new ArgumentNullException("edge");

      this.AddEdge(edge.FromNode, edge.ToNode, edge.Edge);
    }

    /// <summary>
    /// Создание дуги с указанным весом. Если дуга, соединяющая указанную пару вершин уже существует, вес дуги обновляется
    /// </summary>
    /// <param name="fromNode">Начальная вершина в дуге</param>
    /// <param name="toNode">Конечная вершина в дуге</param>
    /// <param name="edge">Вес создаваемой дуги</param>
    /// <returns>True, если дуга была создана. False, если дуга была обновлена</returns>
    public bool SetEdge(TNode fromNode, TNode toNode, TEdge edge)
    {
      using (m_lock.GetWriteLock())
      {
        Dictionary<TNode, TEdge> from_edges;
        Dictionary<TNode, TEdge> to_edges;
        this.FindAdjacency(fromNode, toNode, out from_edges, out to_edges);

        m_edges = null;

        if (from_edges.ContainsKey(toNode))
        {
          from_edges[toNode] = edge;

          if (!m_oriented)
            to_edges[fromNode] = edge;

          return false;
        }
        else
        {
          from_edges.Add(toNode, edge);

          if (!m_oriented)
            to_edges.Add(fromNode, edge);

          return true;
        }
      }
    }

    /// <summary>
    /// Создание дуги с указанным весом. Если дуга, соединяющая указанную пару вершин уже существует, вес дуги обновляется
    /// </summary>
    /// <param name="edge">Сведения о дуге</param>
    /// <returns>True, если дуга была создана. False, если дуга была обновлена</returns>
    public bool SetEdge(EdgeInfo<TNode, TEdge> edge)
    {
      if (edge == null)
        throw new ArgumentNullException("edge");

      return this.SetEdge(edge.FromNode, edge.ToNode, edge.Edge);
    }

    /// <summary>
    /// Удаление ребра
    /// </summary>
    /// <param name="fromNode">Начальная вершиина ребра</param>
    /// <param name="toNode">Конечная вершаина ребра</param>
    /// <returns>True, если ребро было удалено. False, если ребро не найдено</returns>
    public bool RemoveEdge(TNode fromNode, TNode toNode)
    {
      using (m_lock.GetWriteLock())
      {
        Dictionary<TNode, TEdge> from_edges;
        Dictionary<TNode, TEdge> to_edges;
        this.FindAdjacency(fromNode, toNode, out from_edges, out to_edges);

        m_edges = null;

        if (m_oriented)
        {
          return from_edges.Remove(fromNode);
        }
        else
        {
          return from_edges.Remove(fromNode) && to_edges.Remove(toNode);
        }
      }
    }

    /// <summary>
    /// Удаление дуги
    /// </summary>
    /// <param name="edge">Сведения о дуге</param>
    /// <returns>True, если ребро было удалено. False, если ребро не найдено</returns>
    public bool RemoveEdge(EdgeInfo<TNode, TEdge> edge)
    {
      if (edge == null)
        throw new ArgumentNullException("edge");

      return this.RemoveEdge(edge.FromNode, edge.ToNode);
    }

    /// <summary>
    ///  Очистка списка дуг
    /// </summary>
    public void ClearEdges()
    {
      using (m_lock.GetWriteLock())
      {
        foreach (var kv in m_data)
          kv.Value.Clear();

        m_edges.Clear();
      }
    }

    /// <summary>
    /// Полная очистка графа
    /// </summary>
    public void Clear()
    {
      using (m_lock.GetWriteLock())
      {
        m_data.Clear();
        m_edges.Clear();
      }
    }

    /// <summary>
    /// Проверяет наличие ребра в графе
    /// </summary>
    /// <param name="fromNode">Начальная вершиина ребра</param>
    /// <param name="toNode">Конечная вершаина ребра</param>
    /// <returns>True, если ребро найдено. Иначе, False</returns>
    public bool ContainsEdge(TNode fromNode, TNode toNode)
    {
      using (m_lock.GetReadLock())
      {
        Dictionary<TNode, TEdge> from_edges;
        Dictionary<TNode, TEdge> to_edges;

        if (!m_data.TryGetValue(fromNode, out from_edges) || !m_data.TryGetValue(toNode, out to_edges))
          return false;

        return from_edges.ContainsKey(toNode);
      }
    }

    /// <summary>
    /// Возвращает количество рёбер, исходящих из указанной вершины
    /// </summary>
    /// <param name="node">Вершина</param>
    /// <returns>Количество рёбер</returns>
    public int GetOutgoingEdgeCount(TNode node)
    {
      using (m_lock.GetReadLock())
      {
        return m_data[node].Count;
      }
    }

    /// <summary>
    /// Возвращает количество рёбер, входящих в указанную вершину
    /// </summary>
    /// <param name="node">Вершина</param>
    /// <returns>Количество рёбер</returns>
    public int GetIncomingEdgeCount(TNode node)
    {
      using (m_lock.GetReadLock())
      {
        if (m_oriented)
        {
          return m_data.Count(kv => kv.Value.ContainsKey(node));
        }
        else
        {
          // Для неориентированного графа количество исходящих рёбер равно количеству входящих
          return m_data[node].Count;
        }
      }
    }

    /// <summary>
    /// Обращение рёбер ориентированного графа
    /// </summary>
    /// <returns>Новый граф с обращёнными рёбрами</returns>
    public AdjacencyList<TNode, TEdge> InvertDirections()
    {
      using (m_lock.GetReadLock())
      {
        var ret = new AdjacencyList<TNode, TEdge>(m_oriented);

        if (!m_oriented)
        {
          // Для неориентированного графа операция эквивалентна копированию
          foreach (var kv in m_data)
          {
            var value = new Dictionary<TNode, TEdge>();

            ret.m_data.Add(kv.Key, value);

            foreach (var edge in kv.Value)
              value.Add(edge.Key, edge.Value);
          }
        }
        else
        {
          // Формируем список вершин
          foreach (var kv in m_data)
            ret.m_data.Add(kv.Key, new Dictionary<TNode, TEdge>());

          // Просматриваем все дуги исходного графа
          // и добавляем их к вершинам
          foreach (var kv in m_data)
          {
            foreach (var edge in kv.Value)
              ret.m_data[edge.Key].Add(kv.Key, edge.Value);
          }
        }

        return ret;
      }
    }

    /// <summary>
    ///  Получение квадрата ориентированного графа
    /// </summary>
    /// <returns>Граф, являющийся квадратом ориентированного графа</returns>
    public AdjacencyList<TNode, TEdge> Square()
    {
      if (!m_oriented)
        throw new InvalidOperationException();

      using (m_lock.GetReadLock())
      {
        var ret = new AdjacencyList<TNode, TEdge>(m_oriented);

        foreach (var kv in m_data)
        {
          var value = new Dictionary<TNode, TEdge>();

          ret.m_data.Add(kv.Key, value);

          foreach (var edge in kv.Value)
          {
            value.Add(edge.Key, edge.Value);

            foreach (var sec in m_data[edge.Key])
            {
              var new_edge = object.Equals(sec.Value, edge.Value) ? edge.Value : default(TEdge);

              value[sec.Key] = new_edge;
            }
          }
        }
        return ret;
      }
    }

    /// <summary>
    /// Получение цепочек, исходящих из определённой вершины, в виде новых графов
    /// </summary>
    /// <param name="startNode">Исходная вершина</param>
    /// <returns>Список цепочек, исходящих из указанной вершины</returns>
    public List<AdjacencyList<TNode, TEdge>> GetOutgoingChains(TNode startNode)
    {
      using (m_lock.GetReadLock())
      {
        var ret = new List<AdjacencyList<TNode, TEdge>>();
        var part = this.GetOutgoingChainNodes(startNode);

        // Преобразуем временную структуру данных в новые графы
        foreach (var item in part)
        {
          var res = new AdjacencyList<TNode, TEdge>(m_oriented);

          for (int i = 0; i < item.Count; i++)
          {
            res.AddNode(item[i]);

            if (i > 0)
              res.AddEdge(item[i - 1], item[i], m_data[item[i - 1]][item[i]]);
          }
          ret.Add(res);
        }

        return ret;
      }
    }

    /// <summary>
    /// Получение всех цепочек, исходящих из определённой вершины, в виде списков исходящих вершин
    /// </summary>
    /// <param name="startNode">Исходная вершина</param>
    /// <returns>Список цепочек вершин, исходящих из указанной вершины</returns>
    public List<List<TNode>> GetOutgoingChainNodes(TNode startNode)
    {
      return this.GetOutgoingChainNodes(startNode, -1);
    }

    /// <summary>
    /// Получение цепочек, исходящих из определённой вершины, в виде списков исходящих вершин
    /// </summary>
    /// <param name="startNode">Исходная вершина</param>
    /// <param name="maxNodeCount">Максимальное количество узлов, которые может вернуть метод</param>
    /// <returns>Список цепочек вершин, исходящих из указанной вершины</returns>
    public List<List<TNode>> GetOutgoingChainNodes(TNode startNode, int maxNodeCount)
    {
      using (m_lock.GetReadLock())
      {
        var part = new List<List<TNode>>();
        var finishedChains = new HashSet<int>();
        var nodeCount = 1;

        part.Add(new List<TNode> { startNode });

        // Формируем временную структуру данных, содержащую только порядок вершин
        do
        {
          var bifurcations = new List<List<TNode>>();
          for (int i = 0; i < part.Count; i++)
          {
            if (finishedChains.Contains(i))
              continue;

            var sub_list = part[i];

            // Ищем, что ещё можно присоединить к цепочке.
            var out_nodes = m_data[sub_list.Last()].Select(kv => kv.Key).ToList();

            // Цепочка не должна замыкаться сама на себя
            var circles = new HashSet<TNode>(out_nodes.Where(node => sub_list.Contains(node)));
            var next_nodes = out_nodes.Where(node => !circles.Contains(node)).ToList();

            // Если не найдено новых вершин - помечаем цепочку как завершённую
            if (next_nodes.Count == 0)
            {
              finishedChains.Add(i);

              if (out_nodes.Count == 0)
                continue;
            }
            // Иначе добавляем первый найденный узел в конец создаваемой цепочки
            sub_list.Add(out_nodes[0]);
            nodeCount++;

            if (maxNodeCount >= 0 && nodeCount >= maxNodeCount)
            {
              _log.Warn("Node count exceeded limit");
              return part;
            }

            if (circles.Contains(out_nodes[0]))
              finishedChains.Add(i);

            var bif_node_count = nodeCount;

            // Остальные найденные узлы становятся основой для бифуркаций
            for (int j = 1; j < out_nodes.Count; j++)
            {
              // Бифуркацию формируем как копию текущей цепочки,
              // отличающуюся последним элементом
              var copy = new List<TNode>();

              for (int k = 0; k < sub_list.Count - 1; k++)
                copy.Add(sub_list[k]);

              if (circles.Contains(out_nodes[j]))
                finishedChains.Add(part.Count + j - 1);

              copy.Add(out_nodes[j]);

              bif_node_count += copy.Count;

              if (maxNodeCount >= 0 && bif_node_count >= maxNodeCount)
                break;

              bifurcations.Add(copy);
            }
          }

          // Добавляем бифуркации в общий список путей
          foreach (var added in bifurcations)
          {
            nodeCount += added.Count;

            if (maxNodeCount >= 0 && nodeCount >= maxNodeCount)
            {
              _log.Warn("Node count exceeded limit");
              return part;
            }

            part.Add(added);
          }
        }
        // Когда все цепочки считаются законченными, заканчиваем строить
        while (part.Count > finishedChains.Count);
        return part;
      }
    }

    /// <summary>
    /// Выполнение синхронизированной операции над графом
    /// </summary>
    /// <param name="action">Операция, требующая синхронизации</param>
    public void Lock(Action action)
    {
      if (action == null)
        return;

      using (m_lock.GetWriteLock())
      {
        action();
      }
    }
  }

  /// <summary>
  /// Дуга графа
  /// </summary>
  /// <typeparam name="TNode">Тип вершины</typeparam>
  /// <typeparam name="TEdge">Тип дуги</typeparam>
  public class EdgeInfo<TNode, TEdge>
  {
    private readonly TNode m_from_node;
    private readonly TNode m_to_node;
    private readonly TEdge m_edge;

    public EdgeInfo(TNode fromNode, TNode toNode, TEdge edge)
    {
      if (fromNode == null)
        throw new ArgumentNullException("fromNode");

      if (toNode == null)
        throw new ArgumentNullException("toNode");

      m_from_node = fromNode;
      m_to_node = toNode;
      m_edge = edge;
    }

    /// <summary>
    /// Начальная вершина
    /// </summary>
    public TNode FromNode
    {
      get { return m_from_node; }
    }

    /// <summary>
    /// Конечная вершина
    /// </summary>
    public TNode ToNode
    {
      get { return m_to_node; }
    }

    /// <summary>
    /// Дуга
    /// </summary>
    public TEdge Edge
    {
      get { return m_edge; }
    }

    /// <summary>
    /// Обращение дуги
    /// </summary>
    /// <returns>Новая дуга, у которой начальная и конечная вершина поменяны местами</returns>
    public EdgeInfo<TNode, TEdge> Invert()
    {
      return new EdgeInfo<TNode, TEdge>(m_to_node, m_from_node, m_edge);
    }

    public override bool Equals(object obj)
    {
      var other = obj as EdgeInfo<TNode, TEdge>;

      if (other == null)
        return false;

      return other.m_from_node.Equals(m_from_node)
        && other.m_to_node.Equals(m_to_node)
        && object.Equals(other.m_edge, m_edge);
    }

    public override int GetHashCode()
    {
      return m_from_node.GetHashCode() ^ m_to_node.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format("{0} -({1})-> {2}", m_from_node, m_edge, m_to_node);
    }
  }
}
