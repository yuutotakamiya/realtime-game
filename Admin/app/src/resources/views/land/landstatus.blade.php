@extends('layouts.app')
@section('title','島の状況')
@section('h1','島の状況')
@section('body')
    <table class="table table-bordered">
        @if(!empty($landstatus))
            <tr>
                <th>id</th>
                <th>島ID</th>
                <th>ユーザーID</th>
                <th>現在ブロック埋めた数</th>
            </tr>
            @foreach($landstatus as $landstatuses)
                <tr>
                    <td>{{$landstatuses['id']}}</td>
                    <td>{{$landstatuses['land_id']}}</td>
                    <td>{{$landstatuses['user_id']}}</td>
                    <td>{{$landstatuses['land_block_num']}}</td>
                </tr>
            @endforeach
        @endif
    </table>
@endsection



