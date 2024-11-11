@extends('layouts.app')
@section('title','アカウント一覧')
@section('h1','アカウント一覧')

@section('body')
    {{$accounts->links('vendor.pagination.bootstrap-5')}}
    <table class="table table-bordered">
        <tr>
            <td>id</td>
            <td>名前</td>
            <td>パスワード</td>
            <td>操作</td>
        </tr>
        @foreach($accounts as $account)
            <tr>
                <td>{{$account['id']}}</td>
                <td>{{$account['name']}}</td>
                <td>{{$account['password']}}</td>
                <form method="post" action="{{route('accountsaccount_destroy')}}">
                    @csrf
                    <td>
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor"
                             class="bi bi-person-dash"
                             viewBox="0 0 16 16">
                            <path
                                d="M12.5 16a3.5 3.5 0 1 0 0-7 3.5 3.5 0 0 0 0 7ZM11 12h3a.5.5 0 0 1 0 1h-3a.5.5 0 0 1 0-1Zm0-7a3 3 0 1 1-6 0 3 3 0 0 1 6 0ZM8 7a2 2 0 1 0 0-4 2 2 0 0 0 0 4Z"/>
                            <path
                                d="M8.256 14a4.474 4.474 0 0 1-.229-1.004H3c.001-.246.154-.986.832-1.664C4.484 10.68 5.711 10 8 10c.26 0 .507.009.74.025.226-.341.496-.65.804-.918C9.077 9.038 8.564 9 8 9c-5 0-6 3-6 4s1 1 1 1h5.256Z"/>
                        </svg>
                        <button type="submit" onclick="location.href='{{route('accountsaccount_destroy')}}'"
                                name="destroybutton">削除
                        </button>
                        <input type="hidden" name="action" value="destroy">
                        <input type="hidden" name="id" value={{$account['id']}}>
                    </td>
                </form>
                <form method="post" action="{{route('accountspassword_update')}}">
                    @csrf
                    <td>
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor"
                             class="bi bi-person-fill-up" viewBox="0 0 16 16">
                            <path
                                d="M12.5 16a3.5 3.5 0 1 0 0-7 3.5 3.5 0 0 0 0 7Zm.354-5.854 1.5 1.5a.5.5 0 0 1-.708.708L13 11.707V14.5a.5.5 0 0 1-1 0v-2.793l-.646.647a.5.5 0 0 1-.708-.708l1.5-1.5a.5.5 0 0 1 .708 0ZM11 5a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z"/>
                            <path
                                d="M2 13c0 1 1 1 1 1h5.256A4.493 4.493 0 0 1 8 12.5a4.49 4.49 0 0 1 1.544-3.393C9.077 9.038 8.564 9 8 9c-5 0-6 3-6 4Z"/>
                        </svg>
                        <button type="submit" onclick="location.href='{{route('accountspassword_update')}}'"
                                name="destroybutton">更新
                        </button>
                        <input type="hidden" name="action" value="update">
                        <input type="hidden" name="id"
                               value={{$account['id']}} {{$account['name']}} {{$account['password']}}>
                    </td>
                </form>
            </tr>
        @endforeach
    </table>
@endsection
