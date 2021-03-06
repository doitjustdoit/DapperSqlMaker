﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Linq.Expressions;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Collections.Concurrent;

namespace DapperSqlMaker.DapperExt
{

    /// <summary>
    /// 联表类型
    /// </summary>
    public enum JoinType
    {
        Left
        , Right
        , Inner
    }
    public enum SqlClauseType {
        None,
        /// <summary>
        /// MSSql 分页开头语句
        /// </summary>
        PageStartms,
        /// <summary>
        /// MSSql 分页结束语句
        /// </summary>
        PageEndms, 
    }


    /// <summary>
    /// 6表查询
    /// </summary>
    /// <typeparam name="T">主表</typeparam>
    /// <typeparam name="Y">联表2</typeparam>
    /// <typeparam name="Z">联表3</typeparam>
    /// <typeparam name="O">联表4</typeparam>
    /// <typeparam name="P">联表5</typeparam>
    /// <typeparam name="Q">联表6</typeparam>
    public abstract class DapperSqlMaker<T, Y, Z, O, P, Q> : DapperSqlMakerBase<DapperSqlMaker<T, Y, Z, O, P, Q>>
        //where T : class,new()
        //where Y : class, new()
        //where Z : class, new()
        //where O : class, new()
        //where P : class, new()
        //where Q : class, new()
    {
        //public dynamic Models = new { A = new T(), B = new Y(), C = new Z(), D = new O(), E = new P(), F = new Q() };
        public abstract override IDbConnection GetConn();
        public override DapperSqlMaker<T, Y, Z, O, P, Q> GetChild() => this;

        /// <summary>
        /// MSSQL RowRumber OrderBy 字段  p => new { p.Id }  
        /// <para>orderfiesExps必须有返回值 </para>
        /// <para>where子句后不能再拼接orderby子句</para>
        /// <para>生成语句: row_number() over(order by {0}) as rownum, </para>
        /// <para>总记录字段 .Column(p => new { SM.LimitCount, a = SM.Sql("*") })</para>
        /// </summary> 
        public DapperSqlMaker<T, Y, Z, O, P, Q> RowRumberOrderBy(Expression<Func<T, Y, Z, O, P, Q, object>> orderfiesExps)
        {
            LambdaExpression fielambda = orderfiesExps as LambdaExpression;
            if (!(fielambda.Body is NewExpression)) throw new Exception("RowRumber字段未解析");
            return base.RowRumberOrderBy(fielambda);
             
        }
        /// <summary>
        /// 查询指定字段(默认查询*所有字段)  t =>  new { t.Id, t2.Id, x = SM.Sql("*") }
        /// <para>MS Sql总记录字段 .Column(p => new { SM.LimitCount, a = SM.Sql("*") })</para>
        /// </summary> 
        public DapperSqlMaker<T, Y, Z, O, P, Q> Column(Expression<Func<T, Y, Z, O, P, Q, object>> fiesExps = null)
        {
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            return base.Column(fielambda);
             
        }
        public DapperSqlMaker<T, Y, Z, O, P, Q> FromJoin(
            JoinType joinType2, Expression<Func<T, Y, Z, O, P, Q, bool>> joinExps2
          , JoinType joinType3, Expression<Func<T, Y, Z, O, P, Q, bool>> joinExps3
          , JoinType joinType4, Expression<Func<T, Y, Z, O, P, Q, bool>> joinExps4
          , JoinType joinType5, Expression<Func<T, Y, Z, O, P, Q, bool>> joinExps5
          , JoinType joinType6, Expression<Func<T, Y, Z, O, P, Q, bool>> joinExps6)
        {
            //Models.A
            return base.FromJoin(
                      new JoinType[] { joinType2, joinType3, joinType4, joinType5, joinType6 }
            , new LambdaExpression[] { joinExps2, joinExps3, joinExps4, joinExps5, joinExps6 }); 
        }
        public DapperSqlMaker<T, Y, Z, O, P, Q> Where(Expression<Func<T, Y, Z, O, P, Q, bool>> whereExps)
        {
            return base.Where(whereExps); 
        }
        /// <param name="fiesExps"></param>
        /// <param name="isDesc">最后一个字段是否降序</param>
        /// <returns></returns>
        public DapperSqlMaker<T, Y, Z, O, P, Q> Order(Expression<Func<T, Y, Z, O, P, Q, object>> fiesExps, bool isDesc = false)
        {
            return base.Order(fiesExps, isDesc); 
        }

        /// <summary>
        /// 补充自定义DynamicParameters参数
        /// </summary>
        public new DapperSqlMaker<T, Y, Z, O, P, Q> SqlParams(DynamicParameters SqlParams) => base.SqlParams(SqlParams);
    }

    /// <summary>
    /// 5表查询
    /// </summary>
    /// <typeparam name="T">主表</typeparam>
    /// <typeparam name="Y">联表2</typeparam>
    /// <typeparam name="Z">联表3</typeparam>
    /// <typeparam name="O">联表4</typeparam>
    /// <typeparam name="P">联表5</typeparam>
    public abstract class DapperSqlMaker<T, Y, Z, O, P> : DapperSqlMakerBase<DapperSqlMaker<T, Y, Z, O, P>>
    {
        public abstract override IDbConnection GetConn();
        public override DapperSqlMaker<T, Y, Z, O, P> GetChild() => this;
        /// <summary>
        /// MSSQL RowRumber OrderBy 字段  p => new { p.Id }  
        /// <para>orderfiesExps必须有返回值 </para>
        /// <para>where子句后不能再拼接orderby子句</para>
        /// <para>生成语句: row_number() over(order by {0}) as rownum, </para>
        /// <para>总记录字段 .Column(p => new { SM.LimitCount, a = SM.Sql("*") })</para>
        /// </summary> 
        public DapperSqlMaker<T, Y, Z, O, P> RowRumberOrderBy(Expression<Func<T, Y, Z, O, P, object>> orderfiesExps)
        {
            LambdaExpression fielambda = orderfiesExps as LambdaExpression;
            if (!(fielambda.Body is NewExpression)) throw new Exception("RowRumber字段未解析");
            return base.RowRumberOrderBy(fielambda); 
        }
        /// <summary>
        /// 查询指定字段(默认查询*所有字段)  t =>  new { t.Id, t2.Id, x = SM.Sql("*") }
        /// <para>MS Sql总记录字段 .Column(p => new { SM.LimitCount, a = SM.Sql("*") })</para>
        /// </summary> 
        public DapperSqlMaker<T, Y, Z, O, P> Column(Expression<Func<T, Y, Z, O, P, object>> fiesExps = null)
        {
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            return base.Column(fielambda); 
        }
        public DapperSqlMaker<T, Y, Z, O, P> FromJoin(
            JoinType joinType2, Expression<Func<T, Y, Z, O, P, bool>> joinExps2
          , JoinType joinType3, Expression<Func<T, Y, Z, O, P, bool>> joinExps3
          , JoinType joinType4, Expression<Func<T, Y, Z, O, P, bool>> joinExps4
          , JoinType joinType5, Expression<Func<T, Y, Z, O, P, bool>> joinExps5)
        {
            return base.FromJoin(
                      new JoinType[] { joinType2, joinType3, joinType4, joinType5 }
            , new LambdaExpression[] { joinExps2, joinExps3, joinExps4, joinExps5 }); 
        }
        public DapperSqlMaker<T, Y, Z, O, P> Where(Expression<Func<T, Y, Z, O, P, bool>> whereExps)
        {
            return base.Where(whereExps); 
        }
        /// <param name="fiesExps"></param>
        /// <param name="isDesc">最后一个字段是否降序</param>
        /// <returns></returns>
        public DapperSqlMaker<T, Y, Z, O, P> Order(Expression<Func<T, Y, Z, O, P, object>> fiesExps, bool isDesc = false)
        {
            return base.Order(fiesExps, isDesc);
        }
        /// <summary>
        /// 补充自定义DynamicParameters参数
        /// </summary>
        public new DapperSqlMaker<T, Y, Z, O, P> SqlParams(DynamicParameters SqlParams) => base.SqlParams(SqlParams);

    }

    /// <summary>
    /// 4表查询
    /// </summary>
    /// <typeparam name="T">主表</typeparam>
    /// <typeparam name="Y">联表2</typeparam>
    /// <typeparam name="Z">联表3</typeparam>
    /// <typeparam name="O">联表4</typeparam>
    public abstract class DapperSqlMaker<T, Y, Z, O> : DapperSqlMakerBase<DapperSqlMaker<T, Y, Z, O>>
    {
        public abstract override IDbConnection GetConn();
        public override DapperSqlMaker<T, Y, Z, O> GetChild() => this;

        /// <summary>
        /// MSSQL RowRumber OrderBy 字段  p => new { p.Id }  
        /// <para>orderfiesExps必须有返回值 </para>
        /// <para>where子句后不能再拼接orderby子句</para>
        /// <para>生成语句: row_number() over(order by {0}) as rownum, </para>
        /// <para>总记录字段 .Column(p => new { SM.LimitCount, a = SM.Sql("*") })</para>
        /// </summary> 
        public DapperSqlMaker<T, Y, Z, O> RowRumberOrderBy(Expression<Func<T, Y, Z, O, object>> orderfiesExps)
        { 
            LambdaExpression fielambda = orderfiesExps as LambdaExpression;
            if (!(fielambda.Body is NewExpression)) throw new Exception("RowRumber字段未解析");
            return base.RowRumberOrderBy(fielambda); 
        }
        /// <summary>
        /// 查询指定字段(默认查询*所有字段)  t =>  new { t.Id, t2.Id, x = SM.Sql("*") }
        /// <para>MS Sql总记录字段 .Column(p => new { SM.LimitCount, a = SM.Sql("*") })</para>
        /// </summary> 
        public DapperSqlMaker<T, Y, Z, O> Column(Expression<Func<T, Y, Z, O, object>> fiesExps = null)
        {
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            return base.Column(fielambda); 
        }
        public DapperSqlMaker<T, Y, Z, O> FromJoin(
            JoinType joinType2, Expression<Func<T, Y, Z, O, bool>> joinExps2
          , JoinType joinType3, Expression<Func<T, Y, Z, O, bool>> joinExps3
          , JoinType joinType4, Expression<Func<T, Y, Z, O, bool>> joinExps4)
        {
            return base.FromJoin(
                      new JoinType[] { joinType2, joinType3, joinType4 }
            , new LambdaExpression[] { joinExps2, joinExps3, joinExps4 }); 
        }

        public DapperSqlMaker<T, Y, Z, O> Where(Expression<Func<T, Y, Z, O, bool>> whereExps)
        {
            return base.Where(whereExps); 
        }
        /// <param name="fiesExps"></param>
        /// <param name="isDesc">是否降序</param>
        /// <returns></returns>
        public DapperSqlMaker<T, Y, Z, O> Order(Expression<Func<T, Y, Z, O, object>> fiesExps, bool isDesc = false)
        {
            return base.Order(fiesExps, isDesc);
        }
        /// <summary>
        /// 补充自定义DynamicParameters参数
        /// </summary>
        public new DapperSqlMaker<T, Y, Z, O> SqlParams(DynamicParameters SqlParams) => base.SqlParams(SqlParams);

    }

    /// <summary>
    /// 3表查询
    /// </summary>
    /// <typeparam name="T">主表</typeparam>
    /// <typeparam name="Y">联表1</typeparam>
    /// <typeparam name="Z">联表2</typeparam>
    public abstract class DapperSqlMaker<T, Y, Z> : DapperSqlMakerBase<DapperSqlMaker<T, Y, Z>>
    {
        public abstract override IDbConnection GetConn();
        public override DapperSqlMaker<T, Y, Z> GetChild() => this;

        /// <summary>
        /// MSSQL RowRumber OrderBy 字段  p => new { p.Id }  
        /// <para>orderfiesExps必须有返回值 </para>
        /// <para>where子句后不能再拼接orderby子句</para>
        /// <para>生成语句: row_number() over(order by {0}) as rownum, </para>
        /// <para>总记录字段 .Column(p => new { SM.LimitCount, a = SM.Sql("*") })</para>
        /// </summary> 
        public DapperSqlMaker<T, Y, Z> RowRumberOrderBy(Expression<Func<T, Y, Z, object>> orderfiesExps)
        {
            LambdaExpression fielambda = orderfiesExps as LambdaExpression;
            if (!(fielambda.Body is NewExpression)) throw new Exception("RowRumber字段未解析");
            return base.RowRumberOrderBy(fielambda); 
        }
        /// <summary>
        /// 查询指定字段(默认查询*所有字段)  t =>  new { t.Id, t2.Id, x = SM.Sql("*") }
        /// <para>MS Sql总记录字段 .Column(p => new { SM.LimitCount, a = SM.Sql("*") })</para>
        /// </summary>
        public DapperSqlMaker<T, Y, Z> Column(Expression<Func<T, Y, Z, object>> fiesExps = null)
        {
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            return base.Column(fielambda); 
        }
        public DapperSqlMaker<T, Y, Z> FromJoin(
            JoinType joinType2, Expression<Func<T, Y, Z, bool>> joinExps2
            , JoinType joinType3, Expression<Func<T, Y, Z, bool>> joinExps3)
        { 
            return base.FromJoin(
                      new JoinType[] { joinType2, joinType3 }
            , new LambdaExpression[] { joinExps2, joinExps3 }); 
        }

        public DapperSqlMaker<T, Y, Z> Where(Expression<Func<T, Y, Z, bool>> whereExps)
        {
            return base.Where(whereExps); 
        }

        /// <param name="fiesExps"></param>
        /// <param name="isDesc">是否降序</param>
        /// <returns></returns>
        public DapperSqlMaker<T, Y, Z> Order(Expression<Func<T, Y, Z, object>> fiesExps, bool isDesc = false)
        {
            return base.Order(fiesExps, isDesc);
        }
        /// <summary>
        /// 补充自定义DynamicParameters参数
        /// </summary>
        public new DapperSqlMaker<T, Y, Z> SqlParams(DynamicParameters SqlParams) => base.SqlParams(SqlParams);

    }
    /// <summary>
    /// 2表查询
    /// </summary> 
    public abstract class DapperSqlMaker<T, Y> : DapperSqlMakerBase<DapperSqlMaker<T, Y>>
    {
        public abstract override IDbConnection GetConn();
        public override DapperSqlMaker<T, Y> GetChild() => this;

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<dynamic> ExecuteQuery()
        {
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = base.RawSqlParams();

            using (var conn = GetConn())
            {
                var obj = conn.Query<dynamic>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                return obj;
            }
        }
        /// <summary>
        /// MSSQL RowRumber OrderBy 字段  p => new { p.Id }  
        /// <para>orderfiesExps必须有返回值 </para>
        /// <para>where子句后不能再拼接orderby子句</para>
        /// <para>生成语句: row_number() over(order by {0}) as rownum, </para>
        /// <para>总记录字段 .Column(p => new { SM.LimitCount, a = SM.Sql("*") })</para>
        /// </summary> 
        public DapperSqlMaker<T, Y> RowRumberOrderBy(Expression<Func<T, Y, object>> orderfiesExps)
        {
            LambdaExpression fielambda = orderfiesExps as LambdaExpression;
            if (!(fielambda.Body is NewExpression)) throw new Exception("RowRumber字段未解析");
            return base.RowRumberOrderBy(fielambda); 
        }
        /// <summary>
        /// 查询指定字段(默认查询*所有字段)  t =>  new { t.Id, t2.Id, x = SM.Sql("*") }
        /// <para>MS Sql总记录字段 .Column(p => new { SM.LimitCount, a = SM.Sql("*") })</para>
        /// </summary>
        public DapperSqlMaker<T, Y> Column(Expression<Func<T, Y, object>> fiesExps = null)
        {
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            return base.Column(fielambda); 
        }

        /// <summary>
        /// 查询字段连表  待完善扩展列 分页行号 ????
        /// </summary>
        /// <param name="joinType">表连接类型</param>
        /// <param name="joinExps">表连接条件</param>
        /// <returns></returns>
        public DapperSqlMaker<T, Y> FromJoin(JoinType joinType, Expression<Func<T, Y, bool>> joinExps)
        {
            return base.FromJoin(
                      new JoinType[] { joinType }
            , new LambdaExpression[] { joinExps }); 
        }

        //public DapperSqlMaker<T, Y> JoinTable(JoinType joinType, Expression<Func<T, Y, bool>> joinExps)
        //{
        //    // 主表再select中
        //    //var tabname1 = DsmSqlMapperExtensions.GetTableName(typeof(T));  
        //    var tabname2 = DsmSqlMapperExtensions.GetTableName(typeof(Y));

        //    var joinstr = joinType == JoinType.Inner ? " inner join "
        //                  : joinType == JoinType.Left ? " left join "
        //                  : joinType == JoinType.Right ? " right join "
        //                  : null;

        //    LambdaExpression lambda = joinExps as LambdaExpression;
        //    BinaryExpression binaryg = lambda.Body as BinaryExpression;

        //    MemberExpression Member1 = binaryg.Left as MemberExpression;
        //    MemberExpression Member2 = binaryg.Right as MemberExpression;

        //    ParameterExpression Parmexr1 = Member1.Expression as ParameterExpression;
        //    var mberName1 = Parmexr1.Name + "." + Member1.Member.Name;  // 表(别名).字段名

        //    ParameterExpression Parmexr2 = Member2.Expression as ParameterExpression;
        //    var mberName2 = Parmexr2.Name + "." + Member2.Member.Name;  // 表(别名).字段名

        //    var strJoinTable = $" {joinstr} {tabname2} on {mberName1} = {mberName2} ";

        //    Clauses.Add(Clause.New(ClauseType.ActionSelectJoin, jointable: strJoinTable));

        //    return this;
        //}
        //public DapperSqlMaker<T, Y> LeftJoin(Expression<Func<T, Y, bool>> joinExps)
        //{
        //    this.JoinTable(JoinType.Left, joinExps); 
        //    return this;
        //}
        //public DapperSqlMaker<T, Y> RightJoin(Expression<Func<T, Y, bool>> joinExps)
        //{
        //    this.JoinTable(JoinType.Right, joinExps);
        //    return this;
        //}
        //public DapperSqlMaker<T, Y> InnerJoin(Expression<Func<T, Y, bool>> joinExps)
        //{
        //    this.JoinTable(JoinType.Inner, joinExps);
        //    return this;
        //}

        public DapperSqlMaker<T, Y> Where(Expression<Func<T, Y, bool>> whereExps)
        {
            return base.Where(whereExps); 
        }

        // 扩展列 分页行号
        public DapperSqlMaker<T, Y> Order(Expression<Func<T, Y, object>> fiesExps, bool isDesc = false)
        {
            return base.Order(fiesExps, isDesc);
        }
        /// <summary>
        /// 补充自定义DynamicParameters参数
        /// </summary>
        public new DapperSqlMaker<T, Y> SqlParams(DynamicParameters SqlParams) => base.SqlParams(SqlParams);

    }
    /// <summary>
    /// 1表查询
    /// </summary> 
    public abstract class DapperSqlMaker<T> : DapperSqlMakerBase<DapperSqlMaker<T>>
        where T : class, new()
    {
        public abstract override IDbConnection GetConn();
        public override DapperSqlMaker<T> GetChild() => this;

        #region 链式查询数据

        // 查询 
        /// <summary>
        /// MSSQL RowRumber OrderBy 字段  p => new { p.Id }  
        /// <para>orderfiesExps必须有返回值 </para>
        /// <para>where子句后不能再拼接orderby子句</para>
        /// <para>生成语句   row_number() over(order by {0}) as rownum, </para>
        /// <para>总记录字段 .Column(p => new { SM.LimitCount, a = SM.Sql("*") })</para>
        /// </summary> 
        public DapperSqlMaker<T> RowRumberOrderBy(Expression<Func<T, object>> orderfiesExps)
        {
            LambdaExpression fielambda = orderfiesExps as LambdaExpression;
            if (!(fielambda.Body is NewExpression)) throw new Exception("RowRumber字段未解析");
            return base.RowRumberOrderBy(fielambda); 
        }
        /// <summary>
        /// 查询指定字段(默认查询*所有字段)  t =>  new { t.Id, t2.Id, x = SM.Sql("*") }
        /// <para>MS Sql总记录字段 .Column(p => new { SM.LimitCount, a = SM.Sql("*") })</para>
        /// </summary>
        public DapperSqlMaker<T> Column(Expression<Func<T, object>> fiesExps = null)
        {
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            return base.Column(fielambda); 
        }

        /// <summary>
        /// form语句 update和delete时在这一步会增加where关键字防止全表更新操作
        /// </summary>
        /// <returns></returns>
        public DapperSqlMaker<T> From()
        {
            // 1. 存表别名 
            var tabAliasName1 = "a";
            // 3. 主表和查询字段 sel ... from tab
            var tabname1 = DsmSqlMapperExtensions.GetTableName(typeof(T));
            var selstr = $" from {tabname1} {tabAliasName1}"; // sel .. from 表明 别名

            //Clauses.Add(Clause.New(ClauseType.ActionSelectFrom, fromJoin: selstr));
            Clauses.Add(Clause.New(ClauseType.ActionSelectFrom, sql: selstr));
            return this;
        }

        /// <summary>
        /// where语句 select语句where关键字在where()拼接的 所有全表查询可以省略where()子句
        ///          update和delete语句where关键字在from()拼接的 以防止全表更新 全表更新加 1=1条件 
        /// </summary>
        /// <param name="whereExps">查询条件(lp) => lp.Name == lpmodel.Name </param>
        public DapperSqlMaker<T> Where(Expression<Func<T, bool>> whereExps)
        {
            return base.Where(whereExps); 
        }
        /// <summary>
        /// 排序
        /// </summary>
        public DapperSqlMaker<T> Order(Expression<Func<T, object>> fiesExps, bool isDesc = false)
        {
            return base.Order(fiesExps,isDesc); 
        }

        /// <summary>
        /// 补充自定义DynamicParameters参数
        /// </summary>
        public new DapperSqlMaker<T> SqlParamsx(DynamicParameters SqlParams) => base.SqlParams(SqlParams);
        #endregion

        #region 链式 添加数据

        /// <summary>
        /// 链式添加数据 开始标记
        /// </summary>
        public DapperSqlMaker<T> Insert()
        {
            // 1. insert into tab
            var tabname1 = DsmSqlMapperExtensions.GetTableName(typeof(T));
            //Clauses.Add(Clause.New(ClauseType.Insert, insert: " insert into " + tabname1));
            Clauses.Add(Clause.New(ClauseType.Insert, sql: " insert into " + tabname1));
            base.ClauseFirst = ClauseType.Insert;
            return this;
            // $"insert into {name} ({sbColumnList}) values ({sbParameterList})"
        }
        /// <summary>
        /// 要添加的数据列
        /// <para> 栗子: p => new bool[] { p.Id == guid, p.Name == "123"</para>
        /// <para>              , bool SM.Sql("age","(select 1)")</para>
        /// <para>              , bool SM.Sql(p.Age,valsql), bool SM.Sql(varage,varsql) }] </para>
        /// <para> insert output 语句 替换AddColumn语句块的values </para>
        /// <para> .AddColumn().EditClause(ClauseType.AddColumn, "values", " output Inserted.F_Name values ").ExecuteQuery();</para>
        /// <para>.ExecuteQuery()返回output查询列 ExecuteInsert()返回影响行数 </para>
        /// </summary> 
        public DapperSqlMaker<T> AddColumn(Expression<Func<T, bool[]>> fiesExps = null)
        {
            DynamicParameters spars;
            string sqlColmval;
            if (fiesExps == null) throw new Exception("不能执行空的插入语句");
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            base.GetInsertOrUpdateColumnValueStr(fielambda, out spars, out sqlColmval);
            //Clauses.Add(Clause.New(ClauseType.AddColumn, addcolumn: sqlColmval, insertParms: spars));
            Clauses.Add(Clause.New(ClauseType.AddColumn, sql: sqlColmval, parms: spars));
            return this;
        }
        // Insert 影响行数  Insert 最后插入数据Id

        #endregion

        #region 链式 更新数据
        /// <summary>
        /// 链式更新数据 开始标记
        /// </summary>
        /// <returns></returns>
        public DapperSqlMaker<T> Update()
        {
            // 1. insert into tab
            var tabname1 = DsmSqlMapperExtensions.GetTableName(typeof(T));
            //Clauses.Add(Clause.New(ClauseType.Update, update: " update " + tabname1));
            Clauses.Add(Clause.New(ClauseType.Update, sql: " update " + tabname1));
            base.ClauseFirst = ClauseType.Update;
            return this;
            // $" update {name} set {EditColumn}where {where}"
        }
        /// <summary>
        /// 要更新的数据列 
        ///<para>栗子: p => new bool[] { p.Id == guid, p.Name == "123" </para>
        ///<para>           , bool SM.Sql("age","(select 1)")</para>
        ///<para>           , bool SM.Sql(p.Age,valsql), bool SM.Sql(varage,varsql) }</para>
        ///<para>语句结尾会拼接 where 防止全表误操作</para>
        /// </summary> 
        public DapperSqlMaker<T> EditColumn(Expression<Func<T, bool[]>> fiesExps = null)
        {
            DynamicParameters spars;
            string sqlColmval;
            if (fiesExps == null) throw new Exception("不能执行空的插入语句");
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            base.GetInsertOrUpdateColumnValueStr(fielambda, out spars, out sqlColmval, addOrEdit: 2);
            //Clauses.Add(Clause.New(ClauseType.EditColumn, editcolumn: " set " + sqlColmval, updateParms: spars));
            Clauses.Add(Clause.New(ClauseType.EditColumn, sql: " set " + sqlColmval, parms: spars));

            return this;
            // $"insert into {name} ({sbColumnList}) values ({sbParameterList})"
        }
        /// <summary>
        /// 更新列   
        /// <para>update output 需要手动拼接where</para>
        /// <para>.EditColum().SqlClause("output Inserted.F_Name where")</para>
        /// </summary> 
        public DapperSqlMaker<T> EditColum(Expression<Func<T, bool[]>> fiesExps = null)
        {
            DynamicParameters spars;
            string sqlColmval;
            if (fiesExps == null) throw new Exception("不能执行空的插入语句");
            LambdaExpression fielambda = fiesExps as LambdaExpression;
            base.GetInsertOrUpdateColumnValueStr(fielambda, out spars, out sqlColmval, addOrEdit: 2, addwhere: false);
            //Clauses.Add(Clause.New(ClauseType.EditColumn, editcolumn: " set " + sqlColmval, updateParms: spars));
            Clauses.Add(Clause.New(ClauseType.EditColumn, sql: " set " + sqlColmval, parms: spars));
            return this;
        }

        #endregion

        #region 链式 删除数据
        public DapperSqlMaker<T> Delete()
        {
            // 1. insert into tab
            var tabname1 = DsmSqlMapperExtensions.GetTableName(typeof(T));
            // 添加where关键字防止全表删除
            //Clauses.Add(Clause.New(ClauseType.Delete, delete: string.Format(" delete from {0} where ", tabname1)));
            Clauses.Add(Clause.New(ClauseType.Delete, sql: string.Format(" delete from {0} where ", tabname1)));
            base.ClauseFirst = ClauseType.Delete;
            return this;
            // $" delete from {0} where "
        }



        #endregion

    }



    /// <summary>
    /// 语句块类型
    /// </summary>
    public enum ClauseType
    {
        /// <summary>
        /// 查询sql 开始块
        /// </summary>
        ActionSelect,
        //ActionSelectLimitCounts,   
        ActionSelectRowRumberOrderBy,
        ActionSelectColumn,
        ActionSelectFrom,
        //ActionSelectJoin,
        //ActionLeftJoin,
        //ActionRightJoin,
        //ActionInnerJoin,
        ActionSelectWhereOnHaving,
        ActionSelectOrder,
        Table,

        /// <summary>
        /// 新增sql 开始块
        /// </summary>
        Insert,
        /// <summary>
        /// 新增sql 赋值块 
        /// <para> (name1,name2) value(x1,x2)</para>
        /// </summary>
        AddColumn,
        /// <summary>
        /// 修改sql 开始块
        /// </summary>
        Update,
        /// <summary>
        /// 修改sql 赋值块
        /// <para> set name1=x1, name2=x2 where </para>
        /// </summary>
        EditColumn,
        /// <summary>
        /// 删除sql 开始块
        /// </summary>
        Delete,
        SqlClause,
        SqlParams,
    }

    /// <summary>
    /// 拼接语句实体
    /// </summary>
    public class Clause
    {
        public static Clause New(ClauseType type, string sql = null, DynamicParameters parms = null)
        {
            return new Clause
            {
                ClauseType = type,
                ClauseSql = sql,
                ClauseParms = parms
            };
        }
        public ClauseType ClauseType { get; private set; }
        public string ClauseSql { get; set; } // private set; }
        public DynamicParameters ClauseParms { get; set; } // private set; }
         
        //public static Clause New(ClauseType type, string select = null
        //    , string rowRumberOrderBy = null  //, string selectCounts = null
        //    , string selectColumn = null, string fromJoin = null
        //    , string seletTable = null//, string jointable = null, string aliace = null
        //    , string condition = null, DynamicParameters conditionParms = null
        //    , string order = null, string extra = null
        //    , string insert = null, string addcolumn = null, DynamicParameters insertParms = null
        //    , string update = null, string editcolumn = null, DynamicParameters updateParms = null
        //    , string delete = null, string sqlclause = null, DynamicParameters sqlClauseParms = null
        //    ,DynamicParameters sqlParams = null)
        //{
        //    return new Clause
        //    {
        //        ClauseType = type,
        //        Select = select,
        //        //SelectCounts = selectCounts,
        //        SeletTable = seletTable, // 无用
        //        RowRumberOrderBy = rowRumberOrderBy,
        //        SelectColumn = selectColumn,
        //        FromJoin = fromJoin,
        //        //JoinTable = jointable,
        //        //Aliace = aliace,
        //        Condition = condition,
        //        ConditionParms = conditionParms,
        //        Order = order,
        //        Extra = extra,
        //        //添加 ------------
        //        Insert = insert,
        //        AddColumn = addcolumn,
        //        InsertParms = insertParms,
        //        //修改 ------------
        //        Update = update,
        //        EditColumn = editcolumn,
        //        UpdateParms = updateParms,
        //        Delete = delete,
        //        // 任意位置sql
        //        SqlClause = sqlclause,
        //        SqlClauseParms = sqlClauseParms,
        //        SqlParams = sqlParams,
        //    };
        //}



        //public string SeletTable { get; private set; }
        //public string Select { get; private set; }
        ////public string SelectCounts { get; private set; }
        //public string RowRumberOrderBy { get; private set; }
        //public string SelectColumn { get; private set; }
        //public string FromJoin { get; private set; }
        ////public string JoinTable { get; private set; }//
        //public string Condition { get; private set; } // where
        //public string Order { get; private set; }
        ////public string Aliace { get; private set; } 
        //public DynamicParameters ConditionParms { get; private set; }
        //public string Extra { get; private set; } // 字段 
        //public string Insert { get; private set; }
        //public string AddColumn { get; private set; }
        //public DynamicParameters InsertParms { get; private set; }
        //public string Update { get; private set; }
        //public string EditColumn { get; private set; }
        //public DynamicParameters UpdateParms { get; private set; }
        //public DynamicParameters SqlParams { get; private set; }
        //public string Delete { get; private set; }
        ///// <summary>
        ///// 任意位置 sql
        ///// </summary>
        //public string SqlClause { get; private set; }
        ///// <summary>
        ///// 任意位置 sql 参数
        ///// </summary>
        //public DynamicParameters SqlClauseParms { get; private set; }

    }

    /// <summary>
    /// 内部基础方法封装
    /// </summary>
    /// <typeparam name="Child">子类类型</typeparam>
    public abstract class DapperSqlMakerBase<Child> : IDapperSqlMakerBase
    {
        // 当前连接
        public abstract IDbConnection GetConn();
        public abstract Child GetChild();
        public DapperSqlMakerBase()
        {
            //GetConn().Dispose(); //在子类必须重写抽象方法

            //var conn = GetConn();
            //if (conn != null) conn.Dispose();
        }

        #region ISqlAdapter 
        private static readonly ISqlAdapterDsm DefaultAdapter = new SqlServerAdapter();
        private static readonly Dictionary<string, ISqlAdapterDsm> AdapterDictionary
            = new Dictionary<string, ISqlAdapterDsm>
            {
                {"sqlconnection", new SqlServerAdapter()},
                {"sqlceconnection", new SqlCeServerAdapter()},
                {"npgsqlconnection", new PostgresAdapter()},
                {"sqliteconnection", new SQLiteAdapter()},
                {"mysqlconnection", new MySqlAdapter()}, 
            };
        public abstract string GetSqlParamSymbol();
        private static readonly Dictionary<string, string> ParamsDictionary
            = new Dictionary<string, string>
            {
                {"sqlconnection", "@"},
                {"sqlceconnection", "@"},
                {"npgsqlconnection", "@"},
                {"sqliteconnection", "@"},
                {"mysqlconnection", "@"},
                {"oracleconnection",":"}
            };
        protected ISqlAdapterDsm GetSqlAdapter(IDbConnection connection)
        {
            var name = connection.GetType().Name.ToLower();
            return !AdapterDictionary.ContainsKey(name)
                 ? DefaultAdapter
                 : AdapterDictionary[name];
        }
        #endregion

        #region Clause
        protected ClauseType ClauseFirst { get; set; }

          
        protected List<Clause> _clauses;
        protected List<Clause> Clauses
        {
            get { return _clauses ?? (_clauses = new List<Clause>()); }
        }
        /// <summary>
        /// 获取 语句块 实体
        /// </summary>
        /// <param name="ct">语句块类型</param>
        public Clause GetClause(ClauseType ct) => Clauses.FirstOrDefault<Clause>(p => p.ClauseType == ct);
        /// <summary>
        /// 修改 语句块
        /// </summary>
        public DapperSqlMakerBase<Child> EditClause(ClauseType ct,Action<Clause> acf)
        {
            var c = GetClause(ct);
            if (acf != null) acf(c);
            return this;
        }
        /// <summary>
        /// 替换 语句块 sql
        /// </summary>
        /// <param name="ct">语句块类型</param>
        public DapperSqlMakerBase<Child> EditClause(ClauseType ct, string replaceSql, string newsql)
        {
            var c = GetClause(ct);
            c.ClauseSql = c.ClauseSql.Replace(replaceSql, newsql);
            return this;
        }
        #endregion

        #region 表别名字典 缓存数据
        //表别名 ConcurrentDictionary FullName, tabAliasName

        private Dictionary<string, string> _TabAliace;
        /// <summary>
        /// 类全名 + 序号  表别名已经改为按顺序a,b,c,d的形式 FromJoin方法需要使用 where不用了 ？？？？ 看要不要把FromJoin改了
        /// </summary>
        protected Dictionary<string, string> TabAliaceDic
        {
            get { return _TabAliace ?? (_TabAliace = new Dictionary<string, string>()); }
        }
        private List<string> _tabAliasList;
        /// <summary>
        /// 表别名集合
        /// </summary>
        protected List<string> tabAliasList { get { return _tabAliasList ?? (_tabAliasList = new List<string>()); } }

        ///// <summary>
        ///// 实体泛型类 缓存
        ///// </summary>
        //private static readonly ConcurrentDictionary<RuntimeTypeHandle, Type[]> ChildGenericTypes = new ConcurrentDictionary<RuntimeTypeHandle, Type[]>();
        ///// <summary>
        ///// 实体泛型类 缓存
        ///// </summary>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //private static Type[] ChildGenericTypesCache(Type type)
        //{
        //    Type[] pi;
        //    if (ChildGenericTypes.TryGetValue(type.TypeHandle, out pi))
        //    {
        //        return pi;
        //    }
        //    pi = type.GetGenericArguments();
        //    ChildGenericTypes[type.TypeHandle] = pi;
        //    return pi;
        //}

        #endregion

        #region 解析 查询sql
        protected Dictionary<string, int> GetLmdparamsDic(LambdaExpression fielambda)
        {
            Dictionary<string, int> pdic = new Dictionary<string, int>();
            int i = 1;
            foreach (var p in fielambda.Parameters)
            {
                pdic.Add(p.Name, i++);
            }

            return pdic;
        }

        /// <summary>
        /// 生成字段 tab1.Field1, tab1.Field2, tab2.Field3
        /// </summary> 
        /// <param name="fierrExps"></param>
        /// <param name="paramsdic">lmb参数名 序号 字典</param>
        /// <returns></returns>
        protected string GetFieldrrExps(System.Collections.ObjectModel.ReadOnlyCollection<Expression> fierrExps, Dictionary<string, int> paramsdic
            , string[] asnameArr = null) // , System.Collections.ObjectModel.ReadOnlyCollection<System.Reflection.MemberInfo> nmebs = null)
        {

            int i = 0; // 成员序号
            string columns;
            var fiearr = fierrExps.Select(p =>
            {
                bool isfield_suf = asnameArr != null;
                string colum = null;   // 字段
                string field_suf = null; // 字段后缀 (as xxx , desc)
                MemberExpression meb;
                ParameterExpression parmexr;


                if (p.NodeType == ExpressionType.MemberAccess)
                {
                    meb = p as MemberExpression; goto mebexpisnull;
                }
                if (p.NodeType == ExpressionType.Constant)
                {
                    ConstantExpression const1 = p as ConstantExpression;
                    if (p.Type.Name.ToLower() != "string") throw new Exception(p.Type.Name + "常量未解析");
                    colum = const1.Value.ToString();
                    isfield_suf = false; // 直接写sql 别名也要写在字符串里
                    goto appendsql;
                }
                if (p.NodeType == ExpressionType.Convert)
                {
                    UnaryExpression umeb = p as UnaryExpression;
                    meb = umeb.Operand as MemberExpression;
                    goto mebexpisnull; //Constant;
                }
                //;

                if (p.NodeType == ExpressionType.Call)
                {
                    MethodCallExpression method = p as MethodCallExpression;
                    if (method.Method.Name == SM._OrderDesc_Name)
                    { // 倒序字段 order by xx desc
                        meb = method.Arguments[0] as MemberExpression;
                        field_suf = SM.OrderDesc_Sql;
                        goto callstr;
                    }
                    else if (method.Method.Name == SM._Sql_Name) // 插入sql 过时 上面直接判断是否时常量就是
                    {
                        meb = method.Arguments[0] as MemberExpression;
                        if (method.Arguments.FirstOrDefault() is ConstantExpression)
                        { //
                            colum = (method.Arguments.FirstOrDefault() as ConstantExpression).Value.ToString();
                        }
                        else if (method.Arguments.FirstOrDefault() is MemberExpression)
                        { // 值 传入的时 变量 

                            colum = AnalysisExpression.GetMemberValue(method.Arguments.FirstOrDefault() as MemberExpression).ToString();
                        }
                        else throw new Exception(SM._OrderDesc_Name + "未解析");
                        isfield_suf = false; // 直接写sql 别名也要写在字符串里
                        goto appendsql;

                    }
                    else throw new Exception(method.Method.Name + "未解析");
                    //Arguments
                }
                else throw new Exception(p.NodeType + "--" + p + "未解析");



                mebexpisnull:
                if (meb.Expression == null)
                { // 特殊sql
                    string fname = meb.Member.DeclaringType.Name + "." + meb.Member.Name;
                    if (fname == SM._limitcount_Name)
                    {
                        colum = SM.LimitCount;  // 分页Count总记录数字段
                    }
                    else throw new Exception(fname + "未解析");
                    isfield_suf = false; // 已定义的特殊sql 也忽略别名
                    goto appendsql;
                }

                callstr: // 方法直接到这

                parmexr = meb.Expression as ParameterExpression;
                //var tkey = parmexr.Type.FullName + paramsdic[parmexr.Name];  // 类名+参数序号
                //var tabalias1 = tabalis[tkey]; // Parmexr1.Name
                //colum = tabalias1 + "." + meb.Member.Name;  // 表(别名).字段名

                colum = ((char)(paramsdic[parmexr.Name] + 96)) + "." + meb.Member.Name;  // 表(别名).字段名

                appendsql: // 字段名 加  后缀(as xxx, desc)

                if (isfield_suf) field_suf = " as " + asnameArr[i]; // 字段别名
                i++;
                return colum + field_suf;
            }).ToArray<string>();
            columns = " " + string.Join(", ", fiearr) + " ";
            return columns;
        }
        /// <summary>
        /// 生成查询 字段列 
        /// </summary>
        /// <returns></returns>
        protected string GetColumnStr(LambdaExpression fielambda)
        {
            string columns;
            // 2. 解析查询字段
            if (fielambda.Body is NewExpression)
            { // 查询指定字段  // 匿名类型传入Fileds   new { t.f1, t.f2, t2.f3 }   ==>   tab1.f1, tab1.f2, tab2.f3
                Dictionary<string, int> pdic = GetLmdparamsDic(fielambda); //base.
                NewExpression arryexps = fielambda.Body as NewExpression;

                var asnameArr = arryexps.Members.Select(m => m.Name).ToArray<string>();

                columns = GetFieldrrExps(arryexps.Arguments, pdic, asnameArr); //
            }
            else columns = SM.ColumnAll; // 查询所有字段

            return columns;
        }
        /// <summary>
        /// 生成联表 sql
        /// </summary>
        /// <param name="joinType2">联表方式</param>
        /// <param name="tabAliasName2">联表别名</param>
        /// <param name="joinlambda">联表条件表达式</param>
        /// <param name="jointab">联表实体类型</param>
        /// <returns></returns>
        protected string GetJoinTabStr(Type jointab, string tabAliasName2, JoinType joinType2, LambdaExpression joinlambda,ref DynamicParameters spars)
        {
            var joinstr = joinType2 == JoinType.Inner ? " inner join "
                                      : joinType2 == JoinType.Left ? " left join "
                                      : joinType2 == JoinType.Right ? " right join "
                                      : null;

            string tabname2 = DsmSqlMapperExtensions.GetTableName(jointab) + " " + tabAliasName2;  // 表名 别名

            StringBuilder sql = null;
            //DynamicParameters spars = null;

            int aliasIndex = 1;
            Dictionary<string, int> paramsdic = new Dictionary<string, int>();
            foreach (var p in joinlambda.Parameters)
            {
                paramsdic.Add(p.Name, aliasIndex++);
            }

            AnalysisExpression.JoinExpression(joinlambda, ref sql, ref spars, paramsdic: paramsdic, sqlParamSymbol: this.GetSqlParamSymbol(),
              parmSuffix: "_join_"); //sqlMaker.TabAliasName
            string onCondition = sql.ToString();
            var jointabstr = $" {joinstr} {tabname2} on {onCondition} ";
             
            return jointabstr;
        }
        /// <summary>
        /// 生成order sql
        /// </summary>
        /// <param name="fielambda">排序字段 (lp, w) => new { lp.EditCount, lp.Name }</param>
        /// <param name="isDesc">是否降序</param>
        /// <returns></returns>
        protected string GetOrderStr(LambdaExpression fielambda, bool isDesc)
        {

            Dictionary<string, int> pdic = this.GetLmdparamsDic(fielambda);
            string columns;
            NewExpression newexps = fielambda.Body as NewExpression;
            columns = " order by " + GetFieldrrExps(newexps.Arguments, pdic) + (isDesc ? " desc " : null); //base.TabAliasName

            return columns;
        }
        /// <summary>
        /// 生成where sql
        /// </summary>
        /// <param name="whereExps">查询条件(lp, u) => lp.Name == lpmodel.Name || u.UserName == umodel.UserName</param>
        /// <param name="spars">查询条件参数</param>
        /// <param name="sqlCondition">查询条件sql</param>
        protected void GetWhereStr(Expression whereExps, out DynamicParameters spars, out string sqlCondition)
        {
            StringBuilder sql = null;
            spars = null; 

            AnalysisExpression.JoinExpression(whereExps, ref sql, ref spars, isAliasName: this.ClauseFirst == ClauseType.ActionSelect, sqlParamSymbol: this.GetSqlParamSymbol()); //sqlMaker.TabAliasName

            // 防止 update和delete全表操作 已在前面添加where关键字   只有查询语句会执行下行
            if (this.ClauseFirst != ClauseType.Delete && this.ClauseFirst != ClauseType.Update)
                sql.Insert(0, sql.Length > 0 ? " where " : " ");
            sqlCondition = sql.ToString();
        }

        /// <summary>
        /// 任意部分sql拼接
        /// </summary>
        /// <param name="sqlClause"></param>
        /// <param name="index">拼接到指定位置 默认按顺序拼接</param>
        /// <returns></returns>
        private DapperSqlMakerBase<Child> SqlClauseAddClauses(string sqlClause , int index = -1) {
            if (index == -1)
            {
                //Clauses.Add(Clause.New(ClauseType.SqlClause, sqlclause: sqlClause));
                Clauses.Add(Clause.New(ClauseType.SqlClause, sql: sqlClause));
            }
            else { // 不为-1则拼接到 指定位置
       //Clauses.Insert(index, Clause.New(ClauseType.SqlClause, sqlclause: sqlClause));
                 Clauses.Insert(index, Clause.New(ClauseType.SqlClause, sql: sqlClause));
            }
            return this;
        }
        /// <summary>
        /// 特定sql拼接
        /// </summary>
        /// <param name="sqlClauseType">with加分页查询语句首位占位符 默认为空</param>
        /// <returns></returns>
        private DapperSqlMakerBase<Child> SqlClauseTypeAddClauses(int index = -1,SqlClauseType sqlClauseType = SqlClauseType.None)
        {
            if (index == -1)
            {
                //string sqlClause;
                switch (sqlClauseType)
                {
                    case SqlClauseType.PageStartms:
                        //Clauses.Add(Clause.New(ClauseType.SqlClause, sqlclause: SM.PageStartms));
                        Clauses.Add(Clause.New(ClauseType.SqlClause, sql: SM.PageStartms));
                        return this;
                    case SqlClauseType.PageEndms:
                        //Clauses.Add(Clause.New(ClauseType.SqlClause, sqlclause: SM.PageEndms));
                        Clauses.Add(Clause.New(ClauseType.SqlClause, sql: SM.PageEndms));
                        return this;
                    case SqlClauseType.None:
                    default:
                        return this;
                }
            }
            else
            { // 插入到指定位置 Clauses
              //string sqlClause;
                switch (sqlClauseType)
                {
                    case SqlClauseType.PageStartms:
                        //Clauses.Insert(index ,Clause.New(ClauseType.SqlClause, sqlclause: SM.PageStartms)) ;
                        Clauses.Insert(index, Clause.New(ClauseType.SqlClause, sql: SM.PageStartms));
                        return this;
                    case SqlClauseType.PageEndms:
                        //Clauses.Insert(index, Clause.New(ClauseType.SqlClause, sqlclause: SM.PageEndms));
                        Clauses.Insert(index, Clause.New(ClauseType.SqlClause, sql: SM.PageEndms));
                        return this;
                    case SqlClauseType.None:
                    default:
                        return this;
                }
            }
             
        }

        // ####################################################################################

        /// <summary>
        /// 补充自定义DynamicParameters参数
        /// </summary>
        public Child SqlParams(DynamicParameters SqlParams) {
            //Clauses.Add(Clause.New(ClauseType.SqlParams, sqlParams: SqlParams));
            Clauses.Add(Clause.New(ClauseType.SqlParams, parms: SqlParams));
            return this.GetChild();
        }
        /// <summary>
        /// 任意部分sql拼接
        /// </summary>
        /// <param name="sqlClause">拼接语句</param>
        /// <param name="index">拼接到指定位置 默认按顺序拼接</param>
        /// <returns></returns>
        public Child SqlClause(string sqlClause,int index=-1) => this.SqlClauseAddClauses(sqlClause,index).GetChild();
        /// <summary>
        /// MSSql 分页开头语句(配合CTEWith拼接) 拼接内容SM.PageStartms
        /// </summary>
        /// <param name="index">拼接到指定位置 默认按顺序拼接</param>
        /// <returns></returns>
        public Child PageStartms(int index = -1) => this.SqlClauseTypeAddClauses(index,SqlClauseType.PageStartms).GetChild();

        /// <summary>
        /// MSSql 分页结束语句(配合CTEWith拼接) 拼接内容SM.PageEndms
        /// </summary>
        /// <param name="index">拼接到指定位置  默认按顺序拼接</param>
        /// <returns></returns>
        public Child PageEndms(int index = -1) => this.SqlClauseTypeAddClauses(index,SqlClauseType.PageEndms).GetChild();

        /// <summary>
        /// Select 语句头拼接 并记录表别名
        /// <para>.Selec().Column().From()</para>
        /// <para>.Selec().Column().From().Where().Order().ExecuteQuery()</para>
        /// <para>MS Sql分页.Selec().RowRumberOrderBy().Column().From().Where().LoadPagems()</para>
        /// <para>Sqlite分页.Selec().Column().From().Where().Order().LoadPagelt()</para>
        /// </summary>
        /// <returns>子类对象</returns>
        public Child Select()
        {
            Child r = this.GetChild();
            Type rtype = r.GetType(); // 实体泛型类 类型
            //RuntimeTypeHandle rth = rtype.TypeHandle;
            //Type[] ChildTypes = rtype.GetGenericArguments();
            Type[] ChildTypes = DsmSqlMapperExtensions.ChildGenericTypesCache(rtype);
            int tabCount = ChildTypes.Count();
            for (int i = 0; i < tabCount; i++) 
            { // lamada中 参数名-顺序 也重1开始
                var typeFullName = ChildTypes[i].FullName + (i + 1);  // 实体名  +i 防止重复表名键
                var tabAliasName = ((char)(i + 1 + 96)) + "";   // int 97 == 'a'  // 表名 a,b,c...   
                this.TabAliaceDic.Add(typeFullName, tabAliasName);
                this.tabAliasList.Add(tabAliasName);
            }
            //Clauses.Add(Clause.New(ClauseType.ActionSelect, select: " select "));
            Clauses.Add(Clause.New(ClauseType.ActionSelect, sql: " select "));
            return r;
        }
        /// <summary>
        /// MSSQL RowRumber OrderBy 字段  p => p.Id  orderfiesExps必须有返回值 后面的orderby就不要拼接了
        /// </summary> 
        public Child RowRumberOrderBy(LambdaExpression fielambda)
        {
            //LambdaExpression fielambda = orderfiesExps as LambdaExpression;
            if (!(fielambda.Body is NewExpression)) throw new Exception("RowRumber字段未解析");
            // 2. 解析RowRumber_OrderBy字段
            string columns;

            // 查指定字段 
            NewExpression arryexps = fielambda.Body as NewExpression;
            Dictionary<string, int> pdic = this.GetLmdparamsDic(fielambda);
            columns = GetFieldrrExps(arryexps.Arguments, pdic); //"select " +
            columns = string.Format(SM.LimitRowNumber_Sql, columns);

            //Clauses.Add(Clause.New(ClauseType.ActionSelectRowRumberOrderBy, rowRumberOrderBy: columns));
            Clauses.Add(Clause.New(ClauseType.ActionSelectRowRumberOrderBy, sql: columns));
            return this.GetChild();
        }
        /// <summary>
        /// 查询指定字段(默认查询*所有字段) 匿名类型传入Fileds t =>  new { t.f1, t.f2, t2.f3, string SM.Sql() }   ==>   tab1.f1, tab1.f2, tab2.f3
        /// </summary> 
        public Child Column(LambdaExpression fielambda)
        {
            string columns;
            if (/*fiesExps*/fielambda == null) { columns = SM.ColumnAll; goto columnsall; }

            //LambdaExpression fielambda = fiesExps as LambdaExpression;
            columns = this.GetColumnStr(fielambda);

            columnsall:
            Clauses.Add(Clause.New(ClauseType.ActionSelectColumn, sql: columns));
            //Clauses.Add(Clause.New(ClauseType.ActionSelectColumn, selectColumn: columns));
            return this.GetChild();
        }
        public Child FromJoin(  JoinType[] joinTypes, LambdaExpression[] joinExps  )
        {
            //List<string> tabAliasList = new List<string>(); // 表别名集合 a,b,c ...
            if (this.tabAliasList.Count < 2) throw new Exception("FromJoin方法至少需要两表以上的 上下文类调用");
            Child r = this.GetChild();
            Type rtype = r.GetType(); // 实体泛型类 类型
            //RuntimeTypeHandle rth = rtype.TypeHandle;
            //Type[] ChildTypes = rtype.GetGenericArguments();
            Type[] ChildTypes = DsmSqlMapperExtensions.ChildGenericTypesCache(rtype);
             

            StringBuilder sb = new StringBuilder();
            // 2. 主表和查询字段 sel ... from tab
            var tabname1 = DsmSqlMapperExtensions.GetTableName(ChildTypes[0]); // typeof(T));
            /*var selstr = */ sb.AppendFormat( $" from {tabname1} {this.tabAliasList[0]}"); // {tabAliasName1}"; // sel .. from 表明 别名

            DynamicParameters spars = new DynamicParameters();
            for (int i = 1; i < this.tabAliasList.Count; i++)
            {
                //生成联表2,3,4... sql   (join tab2 b on b.x = a.x  
                //                        join tab3 c on c.x = b.x)
                sb.Append( this.GetJoinTabStr(ChildTypes[i], tabAliasList[i], joinTypes[i-1], joinExps[i-1], ref spars) );
            }
            Clauses.Add(Clause.New(ClauseType.ActionSelectFrom, sql: sb.ToString()));
            //Clauses.Add(Clause.New(ClauseType.ActionSelectFrom, fromJoin: sb.ToString() ));
            this.SqlParams(spars); // 补充from join 参数
            return this.GetChild();
             
        }

        /// <summary>
        /// where语句 select语句where关键字在where()拼接的 所有全表查询可以省略where()子句
        ///          update和delete语句where关键字在from()拼接的 以防止全表更新 全表更新加 1=1条件 
        /// </summary>
        /// <param name="whereExps">查询条件(lp, u) => lp.Name == lpmodel.Name || u.UserName == umodel.UserName</param>
        public Child Where(LambdaExpression whereExps)
        {
            DynamicParameters spars;
            string sqlCondition;
            this.GetWhereStr(whereExps, out spars, out sqlCondition);

            //Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, condition: sqlCondition, conditionParms: spars));
            Clauses.Add(Clause.New(ClauseType.ActionSelectWhereOnHaving, sql: sqlCondition, parms: spars));
            return this.GetChild();
        }
        public Child Order(LambdaExpression fielambda, bool isDesc = false)
        {
            //LambdaExpression fielambda = fiesExps as LambdaExpression;
            string columns = this.GetOrderStr(fielambda, isDesc);
            //Clauses.Add(Clause.New(ClauseType.ActionSelectOrder, order: columns));
            Clauses.Add(Clause.New(ClauseType.ActionSelectOrder, sql: columns));
            return this.GetChild();
        }
        #endregion

        #region 解析插入语句sql
        /// <summary>
        /// 生成插入语句 Columns Values
        /// </summary>
        /// <param name="colmvalambda">列和值语法规范p => new object[] { p.Content == p.Name,p.IsDel == false } </param>
        /// <param name="spars">插入语句参数</param>
        /// <param name="sqlColmval">插入语句sql</param>
        /// <param name="addOrEdit">默认新增 1 新增 2 修改</param>
        /// <param name="addwhere">修改是否 拼接where</param>
        protected void GetInsertOrUpdateColumnValueStr(LambdaExpression colmvalambda, out DynamicParameters spars, out string sqlColmval, int addOrEdit = 1, bool addwhere = true)
        {
            //2.解析查询字段
            if (!(colmvalambda.Body is NewArrayExpression)) throw new Exception("不能执行空的插入语句");
            // 列和值语法规范p => new object[] { p.Content == p.Name,p.IsDel == false }   ==>   (Content,IsDel) Value(@Content,@IsDel)
            NewArrayExpression arryexps = colmvalambda.Body as NewArrayExpression;
            StringBuilder sb = new StringBuilder();
            spars = new Dapper.DynamicParameters();
            List<string[]> customColmval = new List<string[]>();
            var lmbdParmName = colmvalambda.Parameters[0].Name;
            int num = 1;
            string exgl = "=";
            if (addOrEdit == 1) sb.Append(" ( "); // 添加输出 修改不用
            foreach (var p in arryexps.Expressions)
            {
                if (p.NodeType == System.Linq.Expressions.ExpressionType.Equal && addOrEdit == 1)
                { //添加
                    AnalysisExpression.BinaryExprssRowSqlParms(p, sb, spars, num++, exgl, (name, parmasName, exglstr) => string.Format("{0},", name)); //" {0} {2} @{0}{1} "
                }
                else if (p.NodeType == System.Linq.Expressions.ExpressionType.Equal && addOrEdit == 2)
                { // 修改
                    AnalysisExpression.BinaryExprssRowSqlParms(p, sb, spars, num++, exgl
                        , (name, parmasName, exglstr) => string.Format(" {0}{2}{3}{1} ,", name, parmasName, exglstr, this.GetSqlParamSymbol() )); //" {0}{2}@{1} ,"//" {0} {2} @{0}{1} "
                }
                else if (p.NodeType == System.Linq.Expressions.ExpressionType.Call)
                {
                    System.Linq.Expressions.MethodCallExpression method = p as System.Linq.Expressions.MethodCallExpression;

                    if (method.Method.Name != SM._Sql_Name) throw new Exception(method.Method.Name + " 暂未做解析的方法 " + p);

                    string[] arrColmval = new string[2]; // 0 column  1 value
                    int i = 0;
                    tempstart:
                    //meb = method.Arguments[0] as System.Linq.Expressions.MemberExpression;
                    if (method.Arguments[i] is System.Linq.Expressions.ConstantExpression)
                    { // Sql()方法内定义的 字符串
                        // 参数名/插入值 直接赋值
                        arrColmval[i] = (method.Arguments[i] as System.Linq.Expressions.ConstantExpression).Value.ToString();
                    }
                    else if (method.Arguments[i] is System.Linq.Expressions.MemberExpression)
                    {// 参数名/插入值 传入的时 变量 

                        var meb = method.Arguments[i] as System.Linq.Expressions.MemberExpression;

                        if (meb.Expression is System.Linq.Expressions.ParameterExpression
                                 && (meb.Expression as System.Linq.Expressions.ParameterExpression).Name == lmbdParmName)
                        { // lambda表达式的参数成员 表示字段参数名 只取成员名称不取值
                            arrColmval[i] = (method.Arguments[i] as System.Linq.Expressions.MemberExpression).Member.Name;
                        }
                        else
                        {// 外部传入的变量
                            arrColmval[i] = AnalysisExpression.GetMemberValue(method.Arguments[i] as System.Linq.Expressions.MemberExpression).ToString();
                        }
                    }
                    //(method.Arguments[i] as System.Linq.Expressions.MemberExpression).Member.Name;
                    else throw new Exception(p.ToString() + " 插入语句未能解析");

                    if (++i < 2) goto tempstart;

                    customColmval.Add(arrColmval);
                }
                else throw new Exception(p.ToString() + " 插入语句未能解析");


            }
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);

            if (addOrEdit == 1)
            { // 添加
                // 拼接子查询插入的 参数名
                sb.Append((spars.ParameterNames.Count() > 0 && customColmval.Count > 0 ? ", " : string.Empty) + string.Join(",", customColmval.Select(p => p[0]).ToList<string>()));

                // 简单参数值 和 子查询
                sb.AppendFormat(") values ({0}{1}) "
                     , string.Join(",", spars.ParameterNames.ToList<string>().Select(p => this.GetSqlParamSymbol() + p).ToList<string>())  // .Select(p => "@" + p)
                     , (spars.ParameterNames.Count() > 0 && customColmval.Count > 0 ? ", " : string.Empty) + string.Join(",", customColmval.Select(p => p[1]).ToList<string>()));
            }
            else if (addOrEdit == 2)
            { // 修改
                // 拼接子查询插入的 参数名
                sb.Append((spars.ParameterNames.Count() > 0 && customColmval.Count > 0 ? ", " : string.Empty) + string.Join(",", customColmval.Select(p => p[0] + "=" + p[1]).ToList<string>()));
                if(addwhere) sb.Append(" where "); // 添加where关键字防止全表操作
                // 简单参数值 和 子查询
                //sb.AppendFormat(") val/*ues ({0}{1}) ", string.Join(",", spars.ParameterNames.ToList<string>().Select(p => "@" + p).ToList<string>())*/
                //     , (spars.ParameterNames.Count() > 0 && customColmval.Count > 0 ? ", " : string.Empty) + string.Join(",", customColmval.Select(p => p[1]).ToList<string>()));
            }

            sqlColmval = sb.ToString();
        }

        #endregion
         
        #region 输出sql执行

        /// <summary>
        /// 生成sql和参数
        /// </summary>
        /// <returns>item1 :sql, item2: 动态参数</returns>
        public Tuple<StringBuilder, DynamicParameters> RawSqlParams()
        {

            if (Clauses.Count == 0)
            {
                throw new Exception("Empty query");
            }
            //var first = Clauses.First();
            if (this.ClauseFirst != ClauseType.ActionSelect && this.ClauseFirst != ClauseType.Insert && this.ClauseFirst != ClauseType.Update && this.ClauseFirst != ClauseType.Delete)
            {
                throw new Exception("Wrong start of query or insert");
            }
            DynamicParameters dparam = new DynamicParameters();
            List<string> columns = new List<string>();
            var sb = new StringBuilder();
            foreach (var clause in Clauses)
            {
                switch (clause.ClauseType)
                {
                    case ClauseType.ActionSelect: // 查询 ----------------
                        sb.Append(clause.ClauseSql); //.Select);
                        break;
                    case ClauseType.ActionSelectRowRumberOrderBy:
                        sb.Append(clause.ClauseSql);//RowRumberOrderBy);
                        break;
                    case ClauseType.ActionSelectColumn:
                        sb.Append(clause.ClauseSql); //SelectColumn);
                        break;
                    case ClauseType.ActionSelectFrom:
                        sb.Append(clause.ClauseSql); //FromJoin);
                        break;
                    case ClauseType.ActionSelectWhereOnHaving:
                        sb.Append(clause.ClauseSql); //Condition);
                        dparam.AddDynamicParams(clause.ClauseParms); //ConditionParms);
                        //if (this.ClauseFirst == ClauseType.ActionSelect)
                        //    dparam.AddDynamicParams(clause.ConditionParms);
                        //else if (this.ClauseFirst == ClauseType.Update)
                        //    dparam.AddDynamicParams(clause.ConditionParms);
                        //else if (this.ClauseFirst == ClauseType.Delete)
                        //    dparam.AddDynamicParams(clause.ConditionParms);
                        break;
                    case ClauseType.ActionSelectOrder:
                        sb.Append(clause.ClauseSql); //Order);
                        break; // --------------查询

                    case ClauseType.Insert: // 新增 -----------------------
                        sb.Append(clause.ClauseSql); //Insert);
                        break;
                    case ClauseType.AddColumn:
                        sb.Append(clause.ClauseSql); //AddColumn);
                        dparam.AddDynamicParams(clause.ClauseParms); //InsertParms);
                        break;// ----------新增

                    case ClauseType.Update: // 更新 -----------------------
                        sb.Append(clause.ClauseSql); //Update);
                        break;
                    case ClauseType.EditColumn:
                        sb.Append(clause.ClauseSql); //EditColumn);
                        dparam.AddDynamicParams(clause.ClauseParms); //UpdateParms);
                        break;// ----------更新 where子句公用select的
                    case ClauseType.Delete: // 删除 -------------------
                        sb.Append(clause.ClauseSql); //Delete);
                        break;// ----------删除 where子句公用select的

                    case ClauseType.SqlClause: //任意部分sql字句拼接
                        sb.Append(clause.ClauseSql); //SqlClause);
                        break;
                    case ClauseType.SqlParams:
                        dparam.AddDynamicParams(clause.ClauseParms); //SqlParams);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            //sb.Append(";");
            if (dparam == null) dparam = new DynamicParameters(); // where 条件为空
            return Tuple.Create(sb, dparam);
        }


        /// <summary>
        /// 生成分页的 sql和参数 
        /// </summary>
        /// <returns>值1 select * from ...  值3 from ...  </returns>
        public Tuple<StringBuilder, DynamicParameters, StringBuilder> RawLimitSqlParams()
        {

            if (Clauses.Count == 0)
            {
                throw new Exception("Empty query");
            }
            var first = Clauses.First();
            if (first.ClauseType != ClauseType.ActionSelect)
            {
                throw new Exception("Wrong start of query");
            }
            DynamicParameters dparam = null;
            var sb = new StringBuilder();
            var countsb = new StringBuilder();
            foreach (var clause in Clauses)
            {
                switch (clause.ClauseType)
                {
                    case ClauseType.ActionSelect:
                        sb.Append(clause.ClauseSql);//Select);
                        //countsb.Append(clause.Select);
                        break;
                    case ClauseType.ActionSelectColumn:
                        sb.Append(clause.ClauseSql); //SelectColumn);  // 查询分页数据  
                        break;
                    case ClauseType.ActionSelectFrom:
                        sb.Append(clause.ClauseSql); //FromJoin);
                        countsb.Append(clause.ClauseSql); //FromJoin);
                        break;
                    case ClauseType.ActionSelectWhereOnHaving:
                        sb.Append(clause.ClauseSql); //Condition);
                        countsb.Append(clause.ClauseSql); //Condition);
                        dparam = clause.ClauseParms; //ConditionParms;
                        break;
                    case ClauseType.ActionSelectOrder:
                        sb.Append(clause.ClauseSql); //Order);
                        countsb.Append(clause.ClauseSql); //Order);
                        break;
                    case ClauseType.SqlClause: //任意部分sql字句拼接
                        sb.Append(clause.ClauseSql); //SqlClause);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            //sb.Append(";");
            if (dparam == null) dparam = new DynamicParameters(); // where 条件为空
            return Tuple.Create(sb, dparam, countsb);
        }

        // 数据库类型 conn.GetType().Name.ToLower()  sqliteconnection
        // "sqlconnection", "sqlceconnection","npgsqlconnection","sqliteconnection","mysqlconnection",

        /// <summary>
        /// 查询多行 dynamic 空数据返回Count=0的IEnumerable&lt;dynamic&gt;
        /// </summary> 
        public virtual IEnumerable<dynamic> ExecuteQuery()
        {
            ////adp = GetSqlAdapter(conn); 
            //ParamsDictionary
            // 循环 sq 和 parms 替换 @ => :

            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();

            using (var conn = GetConn())
            {
                var obj = conn.Query<dynamic>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                return obj;
            }
        }
        /// <summary>
        /// 查询多行 泛型 空数据返回Count=0的IEnumerable&lt;Y&gt;
        /// </summary>
        public virtual IEnumerable<Y> ExecuteQuery<Y>()
        {
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();

            using (var conn = GetConn())
            {
                var obj = conn.Query<Y>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                return obj;
            }
        }

        /// <summary>
        /// 同 ExecuteQuery().ToList()
        /// </summary>
        public virtual List<dynamic> ExecuteQueryList() => this.ExecuteQuery().ToList();
        /// <summary>
        /// 同 ExecuteQuery&lt;Y&gt;().ToList&lt;Y&gt;()
        /// </summary>
        public virtual List<Y> ExecuteQueryList<Y>() => this.ExecuteQuery<Y>().ToList<Y>();

        /// <summary>
        /// 查询首行 泛型 (空数据报错)
        /// </summary>
        public virtual Y ExecuteQueryFirst<Y>()
        {
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();

            using (var conn = GetConn())
            {
                var obj = conn.QueryFirst<Y>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                return obj; //.FirstOrDefault();
            }
        }
        /// <summary>
        /// 查询首行 dynamic  (空数据报错)
        /// </summary>
        /// <returns>dapper封装的dynamic</returns>
        public virtual dynamic ExecuteQueryFirst()
        {
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();

            using (var conn = GetConn())
            {
                var obj = conn.QueryFirst<dynamic>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                return obj;
            }
        }

        /// <summary>
        /// 查询首行 泛型 空数据返回默认值null 同ExecuteQuery&lt;Y&gt;().FirstOrDefault&lt;Y&gt;()
        /// </summary>
        /// <typeparam name="Y"></typeparam>
        /// <returns>Y</returns>
        public virtual Y ExecuteQueryFirstOrDefault<Y>() => this.ExecuteQuery<Y>().FirstOrDefault<Y>();
        /// <summary>
        /// 查询首行 dynamic 空数据返回默认值null 同ExecuteQuery().FirstOrDefault()
        /// </summary> 
        /// <returns>dynamic</returns>
        public virtual dynamic ExecuteQueryFirstOrDefault() => this.ExecuteQuery().FirstOrDefault();

        /// <summary>
        /// 查询首列 T 空数据返回默认值null
        /// </summary>
        public virtual Y ExecuteScalar<Y>()
        {
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();

            using (var conn = GetConn())
            {
                var obj = conn.Query<Y>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                return obj.FirstOrDefault();
            }
        }

        /// <summary>
        /// MSSql CTE分页
        /// </summary> 
        /// <param name="records">总记录数</param>
        public virtual IEnumerable<dynamic> LoadPagemscte(int page, int rows, out int records)
        {
            records = 0;
            ISqlAdapterDsm adp;
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();
            using (var conn = GetConn())
            {
                adp = GetSqlAdapter(conn);

                // 生成分页sql 分页sql之前已经拼接好了 只需要添加分页的几个参数
                adp.RawPage(null, rawSqlParams.Item2, page, rows);
                // 查询分页数据
                var obj = conn.Query<dynamic>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                var first = obj.FirstOrDefault();
                if (first != null) records = int.Parse("0" + first.counts);
                return obj;
            }
        }
        /// <summary>
        /// MSSql分页
        /// </summary>  
        /// <param name="records">总记录数</param>
        public virtual IEnumerable<dynamic> LoadPagems(int page, int rows, out int records)
        {
            records = 0;
            ISqlAdapterDsm adp;
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();
            using (var conn = GetConn())
            {
                adp = GetSqlAdapter(conn);

                // 生成分页sql
                adp.RawPage(rawSqlParams.Item1, rawSqlParams.Item2, page, rows);
                // 查询分页数据
                var obj = conn.Query<dynamic>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                var first = obj.FirstOrDefault();
                if (first != null) records = int.Parse("0" + first.counts);
                return obj;
            }
        }
        /// <summary>
        /// MS Sql分页 T实体需要声明records总记录数字段;
        /// </summary>  
        public virtual IEnumerable<T> LoadPagems<T>(int page, int rows)
        {
            ISqlAdapterDsm adp;
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();
            using (var conn = GetConn())
            {
                adp = GetSqlAdapter(conn);

                // 生成分页sql
                adp.RawPage(rawSqlParams.Item1, rawSqlParams.Item2, page, rows);
                // 查询分页数据
                var obj = conn.Query<T>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                //var first = obj.FirstOrDefault();
                //if (first != null) records = int.Parse("0" + first.counts);
                return obj;
            }
        }
        /// <summary>
        /// MS Sql分页 T实体里声明records接受总记录数;
        /// </summary>
        /// <param name="records">总记录</param>
        /// <param name="getRecords">p => p.records</param>
        /// <returns></returns>
        public virtual IEnumerable<T> LoadPagems<T>(int page, int rows, out int records,Func<T,int> getRecords)
        {
            records = 0;
            var obj = LoadPagems<T>(page,rows);

            var first = obj.FirstOrDefault();
            if (first != null) records = getRecords(first);
            return obj;
        }

        /// <summary>
        ///  MS Sql2012新分页语法(offset / fetch next) T实体里声明records接受总记录数;
        /// </summary>
        /// <param name="records">总记录</param>
        public virtual IEnumerable<dynamic> LoadPageMsSql2<T>(int page, int rows, out int records) {

            records = 0;
            ISqlAdapterDsm adp;
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();
            using (var conn = GetConn())
            {
                adp = GetSqlAdapter(conn);

                //select * from xxxx
                //    offset 8 rows				/* 开始行数  (页面-1)*行数 */
                //    fetch next 5 rows only		/* 行数 */
                // 生成分页sql
                //adp.RawPage(rawSqlParams.Item1, rawSqlParams.Item2, page, rows);
                var offset = (page - 1) * rows;
                rawSqlParams.Item1.Append(" offset @offset rows fetch next @rows rows only");
                rawSqlParams.Item2.Add("offset",offset);
                rawSqlParams.Item2.Add("rows", rows);

                // 查询分页数据
                var obj = conn.Query<dynamic>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                var first = obj.FirstOrDefault();
                if (first != null) records = int.Parse("0" + first.counts);
                return obj;
            }
        }
        /// <summary>
        ///  MS Sql2012新分页   同 LoadPageMsSql2&lt;T&gt;(page, rows, out records).ToList();
        /// </summary>
        /// <param name="records">总记录</param>
        /// <param name="getRecords">p => p.records</param>
        public virtual List<dynamic> LoadPageMsSql2List<T>(int page, int rows, out int records) => LoadPageMsSql2<T>(page, rows, out records).ToList();

        /// <summary>
        ///  MS Sql2012新分页语法(offset / fetch next) T实体里声明records接受总记录数;
        /// </summary>
        /// <param name="records">总记录</param>
        /// <param name="getRecords">p => p.records</param>
        public virtual IEnumerable<T> LoadPageMsSql2<T>(int page, int rows)
        {
            ISqlAdapterDsm adp;
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();
            using (var conn = GetConn())
            {
                adp = GetSqlAdapter(conn);
                var offset = (page - 1) * rows;
                rawSqlParams.Item1.Append(" offset @offset rows fetch next @rows rows only");
                rawSqlParams.Item2.Add("offset", offset);
                rawSqlParams.Item2.Add("rows", rows);
                // 查询分页数据
                var obj = conn.Query<T>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                return obj;
            }
        }
        /// <summary>
        ///  MS Sql2012新分页   同  LoadPageMsSql2&lt;T&gt;(page, rows).ToList&lt;T&gt;()
        /// </summary>
        public virtual List<T> LoadPageMsSql2List<T>(int page, int rows) => LoadPageMsSql2<T>(page, rows).ToList<T>();
        /// <summary>
        ///  MS Sql2012新分页语法(offset / fetch next) T实体里声明records接受总记录数;
        /// </summary>
        /// <param name="records">总记录</param>
        /// <param name="getRecords">p => p.records</param>
        public virtual IEnumerable<T> LoadPageMsSql2<T>(int page, int rows, out int records, Func<T, int> getRecords)
        {
            records = 0;
            var obj = LoadPageMsSql2<T>(page, rows);

            var first = obj.FirstOrDefault();
            if (first != null) records = getRecords(first);
            return obj;
        }
        /// <summary>
        ///  MS Sql2012新分页   同 LoadPageMsSql2&lt;T&gt;(page, rows, out records, getRecords).ToList&lt;T&gt;()
        /// </summary>
        /// <param name="records">总记录</param>
        /// <param name="getRecords">p => p.records</param>
        public virtual List<T> LoadPageMsSql2List<T>(int page, int rows, out int records, Func<T, int> getRecords) => LoadPageMsSql2<T>(page, rows, out records, getRecords).ToList<T>();


        /// <summary>
        /// sqlite分页 dynamic 空数据返回Count=0的IEnumerable&lt;dynamic&gt;
        /// </summary> 
        public virtual IEnumerable<dynamic> LoadPagelt(int page, int rows, out int records)
        {
            ISqlAdapterDsm adp;
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters, StringBuilder> rawSqlParams = this.RawLimitSqlParams();
            using (var conn = GetConn())
            {
                adp = GetSqlAdapter(conn);

                adp.RawPage(rawSqlParams.Item1, rawSqlParams.Item2, page, rows);
                // 查询分页数据
                var obj = conn.Query<dynamic>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);

                rawSqlParams.Item3.Insert(0, SM.LimitSelectCount);
                var objrecords = conn.ExecuteScalar(rawSqlParams.Item3.ToString(), rawSqlParams.Item2);

                // 查询总记录数
                //Clauses.Insert(0, Clause.New(ClauseType.ActionSelectColumn, selectColumn: ));
                records = int.Parse(objrecords.ToString());
                return obj;
            }
        }
        /// <summary>
        /// sqlite分页 T 空数据返回Count=0的IEnumerable&lt;T&gt;
        /// </summary> 
        public virtual IEnumerable<T> LoadPagelt<T>(int page, int rows, out int records)
        {
            ISqlAdapterDsm adp;
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters, StringBuilder> rawSqlParams = this.RawLimitSqlParams();
            using (var conn = GetConn())
            {
                adp = GetSqlAdapter(conn);

                adp.RawPage(rawSqlParams.Item1, rawSqlParams.Item2, page, rows);
                // 查询分页数据
                var obj = conn.Query<T>(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);

                rawSqlParams.Item3.Insert(0, SM.LimitSelectCount);
                var objrecords = conn.ExecuteScalar(rawSqlParams.Item3.ToString(), rawSqlParams.Item2);

                // 查询总记录数
                //Clauses.Insert(0, Clause.New(ClauseType.ActionSelectColumn, selectColumn: ));
                records = int.Parse(objrecords.ToString());
                return obj;
            }
        }


        /// <summary>
        /// 执行添加sql  返回影响行
        /// </summary>
        public virtual int ExecuteInsert()
        {
            // Tuple<sql,entity>
            Tuple<StringBuilder, DynamicParameters> rawSqlParams = this.RawSqlParams();
            var conn = GetConn();
            var s = conn.State; 

            using (conn)
            {
                var obj = conn.Execute(rawSqlParams.Item1.ToString(), rawSqlParams.Item2);
                return obj;
            }
        }
        /// <summary>
        /// 执行修改sql 返回影响行 修改全表操作需写Where条件  
        /// </summary>
        /// <returns></returns>
        public virtual int ExecuteUpdate() => this.ExecuteInsert();
        /// <summary>
        /// 执行删除sql 返回影响行 删除全表操作需写Where条件  
        /// <para>delete output 语句 同insert处理方式 替换Delete语句块的where</para>
        /// </summary>
        public virtual int ExecuteDelete() => this.ExecuteInsert(); 



        #endregion
         
        //public virtual IEnumerable<dynamic> ExecuteQuery() { throw new Exception("子类未重写该方法"); }

    }

    public interface IDapperSqlMakerBase
    {
        /// <summary>
        /// 当前库连接
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConn();
        /// <summary>
        /// 参数化 占位符 "@" ":"
        /// </summary>
        /// <returns></returns>
        string GetSqlParamSymbol();
    }

}
