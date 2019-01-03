**DapperSqlMaker ��ʽ��ѯ��չ** 
###### ����:
	Dapper-1.50.2\Dapper
	Dapper-1.50.2\Dapper.Contrib
###### Demo:
	��ѯ TestsFW.Common\DapperExt\SelectDapperSqlMakerTest.cs
	��� TestsFW.Common\DapperExt\InsertDapperSqlMakerTest.cs
	���� TestsFW.Common\DapperExt\UpdateDapperSqlMakerTest.cs
	ɾ�� TestsFW.Common\DapperExt\DeleteDapperSqlMakerTest.cs
	��������      FW.Common\DapperExt\LockDapperUtilsqlite.cs
	
##### �����ӣ�

###### 1.��ѯ-�����ѯ,��ҳ

```
[Test]
public void ���������ҳ����()
{
    LockPers lpmodel = new LockPers() { Name = "%����%", IsDel = false};
    Users umodel = new Users() { UserName = "jiaojiao" };
    SynNote snmodel = new SynNote() { Name = "%ľͷ%" };
    Expression<Func<LockPers, Users, SynNote, bool>> where = PredicateBuilder.WhereStart<LockPers, Users, SynNote>();
    where = where.And((lpw, uw, sn) => lpw.Name.Contains(lpmodel.Name));
    where = where.And((lpw, uw, sn) => lpw.IsDel == lpmodel.IsDel);
    where = where.And((lpw, uw, sn) => uw.UserName == umodel.UserName);
    where = where.And((lpw, uw, sn) => sn.Name.Contains(snmodel.Name));

    DapperSqlMaker<LockPers, Users, SynNote> query = LockDapperUtilsqlite<LockPers, Users, SynNote>
        .Selec()
        .Column((lp, u, s) =>		// null) //��ѯ�����ֶ�
            new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
        .FromJoin(JoinType.Left, (lpp, uu, snn) => uu.Id == lpp.UserId
                , JoinType.Inner, (lpp, uu, snn) => uu.Id == snn.UserId)
        .Where(where)
        .Order((lp, w, sn) => new { lp.EditCount, lp.Name, sn.Content });

    var result = query.ExcuteSelect(); //1. ִ�в�ѯ
    WriteJson(result); //  ��ѯ���

    Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
    WriteSqlParams(resultsqlparams);

    int page = 2, rows = 3, records;
    var result2 = query.LoadPagelt(page, rows, out records); //2. ��ҳ��ѯ
    WriteJson(result2); //  ��ѯ���
}
```
##### 2.����-���²����ֶ�

```
[Test]
public void ���²����ֶβ���lt()
{
    var issucs = LockDapperUtilsqlite<LockPers>.Cud.Update(
        s =>
        {
            s.Name = "����bool�޸�1";
            s.Content = "update�����ڸ�ֵ�޸��ֶ�";
            s.IsDel = true;
        },
        w => w.Name == "����bool�޸�1" && w.IsDel == true
        );
    Console.WriteLine(issucs);
}
```



> //########################################
> �齱���� Demo
> sqlite ��
> 
> ע�⣺
> 1. svn�ύ��githubʱ ��Ҫ�ٽ�������ڸ����ļ� ֱ�ӵ������ļ���ӽ��� 
> 
> 
> ��ӡ��ѯsql
> ����: Dapper.SqlMapper.QueryImpl
> ȡ��ע��: // Console.WriteLine(cmd.CommandText);  
> 
> where����
> �ɱ���� �Ƚ�ʱ ��ת��ֵ����
> 
> ʵ�������׺��Ҫ������