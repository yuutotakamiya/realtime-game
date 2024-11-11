@extends('layouts.app')
@section('title','ユーザー所持アイテム一覧')
@section('h1','ユーザー所持アイテム一覧')
@section('body')
    <form method="get" action="{{route('accounts.useritemList')}}">
        @csrf
        <div class="search">
            <input type="search" id="search-text" name="id" class="searchform"
                   placeholder="idを入力">
            <button id="searchBtn">検索</button>
            <input type="hidden" name="action" value="{{$items}}">
        </div>
    </form>
    @if(!empty($items))
        {{$items->links('vendor.pagination.bootstrap-5')}}
        <table class="table table-bordered">
            <tr>
                <th>id</th>
                <th>プレイヤーの名前</th>
                <th>アイテムの名前</th>
                <th>所持個数</th>
            </tr>
            @foreach($user as $users)
                @foreach($items as $item)
                    <tr>
                        <td>{{$item['id']}}</td>
                        <td>{{$user->name}}</td>
                        <td>{{$item->name}}</td>
                        <td>{{$item->pivot->Quantity_in_possession}}</td>
                    </tr>
                @endforeach
            @endforeach
        </table>
    @endif
@endsection


