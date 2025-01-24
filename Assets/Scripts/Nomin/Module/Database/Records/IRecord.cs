/// <summary>
/// <br>초기화 레코드 (Scriptable Object) 공통 규약</br>
/// <br>CONSTRAINT : 상속받는 레코드에서 타겟 테이블의 모든 필드를 구현해야 합니다.</br>
/// <br>CONSTRAINT : 테이블의 필드 순서와 정확히 일치해야 합니다.</br>
/// </summary>
public interface IRecord
{
    public string ID { get; set; }
}