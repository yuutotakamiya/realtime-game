@extends('layouts.app')
@section('title','アイテム一覧')
@section('h1','アイテム一覧')
@section('body')
    {{$accounts->links('vendor.pagination.bootstrap-5')}}
    <table class="table table-bordered">
        <tr>
            <th>id</th>
            <th>名前</th>
            <th>種別</th>
            <th>効果値</th>
            <th>説明</th>
        </tr>
        @foreach($accounts as $itemList)
            <tr>
                <td>{{$itemList['id']}}</td>
                <td>{{$itemList['name']}}</td>
                <td>{{$itemList['type']}}</td>
                <td>{{$itemList['effect_size']}}</td>
                <td>{{$itemList['Description']}}</td>
            </tr>
        @endforeach
    </table>
@endsection


