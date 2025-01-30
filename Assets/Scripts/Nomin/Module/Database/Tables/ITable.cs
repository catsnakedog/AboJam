using System;
using System.Collections.Generic;

/// <summary>
/// <br>런타임 데이터베이스의 테이블 입니다.</br>
/// <br>CONSTRAINT : TableName 은 서버 테이블 이름과 정확히 동일해야합니다.</br>
/// </summary>
public interface ITable
{
    /// <summary>
    /// 반드시 서버 테이블 이름과 동일해야 합니다.
    /// </summary>
    public string TableName { get; set; }
    public string ID { get; set; }
}