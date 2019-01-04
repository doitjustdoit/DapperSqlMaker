**DapperSqlMaker ��ʽ��ѯ��չ** 

[Gihub��ַ](https://github.com/mumumutou/DapperSqlMaker)
###### Nuget��װ:  
	Install-Package DapperSqlMaker -Version 0.0.1

###### ����(������Դ��):
	Dapper-1.50.2\Dapper
	Dapper-1.50.2\Dapper.Contrib
###### Demo:
	��ѯ       TestsDapperSqlMaker\DapperSqlMaker.Test\  SelectDapperSqlMakerTest.cs
	���       TestsDapperSqlMaker\DapperSqlMaker.Test\  InsertDapperSqlMakerTest.cs
	����       TestsDapperSqlMaker\DapperSqlMaker.Test\  UpdateDapperSqlMakerTest.cs
	ɾ��       TestsDapperSqlMaker\DapperSqlMaker.Test\  DeleteDapperSqlMakerTest.cs
	��������   TestsDapperSqlMaker\DbDapperSqlMaker\     LockDapperUtilsqlite.cs
	
##### �����ӣ�

##### 1.��ѯ-�����ѯ,��ҳ

```csharp
[Test]
        public void ���������ҳ����()
        {
            string uall = "b.*";
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
                .Column((lp, u, s) => //null)  //��ѯ�����ֶ�
                    new { lp.Name, lpid = lp.Id, a = "LENGTH(a.Prompt) as plen", b = SM.Sql(uall), scontent = s.Content, sname = s.Name })
                .FromJoin(JoinType.Left, (lpp, uu, snn) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn) => uu.Id == snn.UserId)
                .Where(where)
                .Order((lp, w, sn) => new { lp.EditCount, lp.Name, sn.Content });

            var result = query.ExcuteSelect();
            WriteJson(result); //  ��ѯ���

            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            WriteSqlParams(resultsqlparams); // ��ӡsql�Ͳ���

            int page = 2, rows = 3, records;
            var result2 = query.LoadPagelt(page, rows, out records);
            WriteJson(result2); //  ��ѯ���
        }
```
*���ɵ�sql* :
```sql
 select  a.Name as Name, a.Id as lpid
	, LENGTH(a.Prompt) as plen, b.*
	, c.Content as a, c.Name as b  
	from LockPers a  
	left join  Users b on  b.Id = a.UserId   
	inner join  SynNote c on  b.Id = c.UserId  
	where  a.Name like @Name0  and  a.IsDel = @IsDel1  
		   and  b.UserName = @UserName2  and  c.Name like @Name3  
    order by  a.EditCount, a.Name, c.Content 
```

##### 2.����-���²����ֶ�

```csharp
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


 

-----

 
 ע�⣺
> 1. svn�ύ��githubʱ ��Ҫ�ٽ�������ڸ����ļ� ֱ�ӵ������ļ���ӽ��� 
> 2. ���Դ�ӡdapper��ѯsql  
   ����: Dapper.SqlMapper.QueryImpl  
    ȡ��ע��: // Console.WriteLine(cmd.CommandText);  
> 3. where����
     �ɱ���� �Ƚ�ʱ ��ת��ֵ����
> 4. ʵ�������׺��Ҫ������
> 5. ������� a,b,c... ˳������
> 6. ���������ϴ���չ       
    ֻ��copy��ʵ�ֵ� �޸�3���ļ�            
    DapperSqlMaker              
    Template_DapperSqlMaker ��������         
    PredicateBuilder        ����ƴ����
> 7. [ʵ������T4ģ��ʹ�÷�������](https://www.cnblogs.com/cl-blogs/p/7205954.html)
>
