#if UNITY_EDITOR
public class CodeFormat 
{
    public static string CharacterFormat = 
@"using UnityEngine;

public class {0} : MonoBehaviour
{{
    private const string _name = ""{1}"";
    private CharacterType _type = CharacterType.{2};

    public void Introduce()
    {{
        Debug.Log($""{{_name}} �̰� {{_type}} �Դϴ�."");
    }}
}}
";
}
#endif

// �ٲ� �κп� ���� �ְ�
// �� ���� ������� ���� ��